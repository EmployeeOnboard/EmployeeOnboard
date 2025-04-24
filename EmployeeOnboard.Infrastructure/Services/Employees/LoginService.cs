using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces;
using EmployeeOnboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EmployeeOnboard.Infrastructure.Data;
using System.Security.Authentication;

namespace EmployeeOnboard.Infrastructure.Services.Employees
{
    public class LoginService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginService> _logger;

        public LoginService(ApplicationDbContext context, IConfiguration configuration, ILogger<LoginService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDTO)
        {
            try
            {
                var user = await GetEmployeeByEmailAsync(loginDTO.Email);

                //if (user == null || !IsPasswordValid(loginDTO.Password, user.Password))
                //{
                //    _logger.LogWarning("Invalid login attempt for email: {Email}", loginDTO.Email);
                //    throw new AuthenticationException("Invalid credentials");
                //}

                if (user == null || !IsPasswordValid(loginDTO.Password, user.Password))
                {
                    _logger.LogWarning("Invalid login attempt for email: {Email}", loginDTO.Email);
                    return new AuthResponseDTO
                    {
                        Success = false,
                        Message = "Invalid credentials"
                    };
                }


                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                await SaveOrUpdateRefreshTokenAsync(user, refreshToken);

                return new AuthResponseDTO
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    Message = "Successfully logged in",
                    Success = true
                };
            }
            catch (AuthenticationException)
            {
                throw; // Bubble up to controller
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during login for email: {Email}", loginDTO.Email);
                throw new Exception("An error occurred while processing your login. Please try again later.");
            }
        }

        private async Task<Employee?> GetEmployeeByEmailAsync(string email)
        {
            return await _context.Employees.FirstOrDefaultAsync(u => u.Email == email);
        }

        private bool IsPasswordValid(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }

        private string GenerateJwtToken(Employee employee)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, employee.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, employee.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        private async Task SaveOrUpdateRefreshTokenAsync(Employee user, string refreshToken)
        {
            if (user.RefreshToken != null)
            {
                user.RefreshToken.Token = refreshToken;
                user.RefreshToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
            }
            else
            {
                user.RefreshToken = new RefreshToken
                {
                    Token = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    EmployeeId = user.Id
                };
                await _context.RefreshTokens.AddAsync(user.RefreshToken);
            }

            await _context.SaveChangesAsync();
        }
    }
}
