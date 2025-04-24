
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Text;
//using EmployeeOnboard.Application.DTOs;
//using EmployeeOnboard.Application.Interfaces;
//using EmployeeOnboard.Domain.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using EmployeeOnboard.Infrastructure.Data;


//namespace EmployeeOnboard.Infrastructure.Services.Employees
//{
//    public class LoginService : IAuthService
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IConfiguration _configuration;
//        private readonly ILogger<LoginService> _logger;

//        public LoginService(ApplicationDbContext context, IConfiguration configuration, ILogger<LoginService> logger)
//        {
//            _context = context;
//            _configuration = configuration;
//            _logger = logger;
//        }

//        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDTO)
//        {
//            var user = await _context.Employees
//                .FirstOrDefaultAsync(u => u.Email == loginDTO.Email);


//            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
//            {
//                return new AuthResponseDTO
//                {
//                    Message = "Invalid credentials",
//                    Success = false
//                };
//            }

//            string GenerateRefreshToken()
//            {
//                var randomBytes = new byte[64];
//                using (var rng = RandomNumberGenerator.Create())
//                {
//                    rng.GetBytes(randomBytes);
//                    return Convert.ToBase64String(randomBytes);
//                }
//            }

//            string GenerateJwtToken(Employee employee)
//            {
//                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
//                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//                var claims = new[]
//                {
//                    new Claim(JwtRegisteredClaimNames.Sub, employee.Id.ToString()),
//                    new Claim(JwtRegisteredClaimNames.Email, employee.Email),
//                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
//                    new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString())
//                };

//                var token = new JwtSecurityToken(
//                    issuer: _configuration["Jwt:Issuer"],
//                    audience: _configuration["Jwt:Audience"],
//                    claims: claims,
//                    expires: DateTime.UtcNow.AddHours(2),
//                    signingCredentials: credentials
//                );

//                return new JwtSecurityTokenHandler().WriteToken(token);
//            }



//            var token = GenerateJwtToken(user);
//            var refreshToken = GenerateRefreshToken();

//            if (user.RefreshToken != null)
//            {
//                // Update existing refresh token
//                user.RefreshToken.Token = refreshToken;
//                user.RefreshToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
//            }
//            else
//            {
//                // Create new refresh token
//                user.RefreshToken = new RefreshToken
//                {
//                    Token = refreshToken,
//                    ExpiresAt = DateTime.UtcNow.AddDays(7),
//                    EmployeeId = user.Id
//                };
//                await _context.RefreshTokens.AddAsync(user.RefreshToken);
//            }

//            await _context.SaveChangesAsync();

//            return new AuthResponseDTO
//            {
//                Token = token,
//                RefreshToken = refreshToken,
//                Message = "Successfully logged in",
//                Success = true
//            };
//        }

//    }
//}

























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
                var user = await ValidateUserAsync(loginDTO.Email, loginDTO.Password);
                if (user == null)
                {
                    return new AuthResponseDTO
                    {
                        Message = "Invalid credentials",
                        Success = false
                    };
                }

                var jwtToken = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                await StoreRefreshTokenAsync(user, refreshToken);

                return new AuthResponseDTO
                {
                    Token = jwtToken,
                    RefreshToken = refreshToken,
                    Message = "Successfully logged in",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");
                return new AuthResponseDTO
                {
                    Message = "An unexpected error occurred",
                    Success = false
                };
            }
        }

        // === SRP-compliant private methods ===

        private async Task<Employee?> ValidateUserAsync(string email, string password)
        {
            var user = await _context.Employees.FirstOrDefaultAsync(u => u.Email == email);
            return (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password)) ? user : null;
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
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private async Task StoreRefreshTokenAsync(Employee user, string refreshToken)
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


