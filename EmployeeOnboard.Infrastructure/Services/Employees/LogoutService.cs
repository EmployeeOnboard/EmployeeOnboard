


//using EmployeeOnboard.Application.Interfaces;
//using Microsoft.EntityFrameworkCore;
//using EmployeeOnboard.Infrastructure.Data;
//using Microsoft.Extensions.Logging;
//using EmployeeOnboard.Domain.Entities;

//namespace EmployeeOnboard.Infrastructure.Services.Employees
//{
//    public class LogoutService : ILogoutService
//    {
//        private readonly ApplicationDbContext _context;

//        public LogoutService(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<bool> LogoutAsync(Guid userId)
//        {
//            try
//            {
//                var user = await GetUserByIdAsync(userId);
//                if (user == null)
//                {
//                    return false;
//                }

//                var tokensRemoved = await RemoveRefreshTokensAsync(user.Id);
//                return tokensRemoved;
//            }
//            catch (Exception)
//            {
//                return false;
//            }
//        }

//        // === Private methods for core functionality ===

//        private async Task<Employee?> GetUserByIdAsync(Guid userId)
//        {
//            return await _context.Employees.FirstOrDefaultAsync(u => u.Id == userId);
//        }

//        private async Task<bool> RemoveRefreshTokensAsync(Guid userId)
//        {
//            var refreshTokens = await _context.RefreshTokens
//                .Where(rt => rt.EmployeeId == userId)
//                .ToListAsync();

//            if (refreshTokens.Any())
//            {
//                _context.RefreshTokens.RemoveRange(refreshTokens);
//                await _context.SaveChangesAsync();
//                return true;
//            }

//            return false;
//        }
//    }
//}






















using EmployeeOnboard.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using EmployeeOnboard.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using EmployeeOnboard.Domain.Entities;
using System.Security.Claims;

namespace EmployeeOnboard.Infrastructure.Services.Employees
{
    public class LogoutService : ILogoutService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LogoutService> _logger;

        public LogoutService(ApplicationDbContext context, ILogger<LogoutService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool Success, string Message)> LogoutAsync(ClaimsPrincipal user)
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                if (userId == null)
                {
                    return (false, "Invalid user");
                }

                var employee = await GetEmployeeAsync(userId.Value);
                if (employee == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found during logout", userId);
                    return (false, "User not found");
                }

                await RemoveRefreshTokensAsync(userId.Value);

                return (true, "Logged out successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
                return (false, "An error occurred during logout");
            }
        }

        // 🔹 Extracts userId from claims
        private Guid? GetUserIdFromClaims(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return null;
            }
            return userId;
        }

        // 🔹 Retrieves the employee entity
        private async Task<Employee?> GetEmployeeAsync(Guid userId)
        {
            return await _context.Employees.FirstOrDefaultAsync(u => u.Id == userId);
        }

        // 🔹 Removes the refresh tokens for the employee
        private async Task RemoveRefreshTokensAsync(Guid userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(rt => rt.EmployeeId == userId)
                .ToListAsync();

            if (tokens.Any())
            {
                _context.RefreshTokens.RemoveRange(tokens);
                await _context.SaveChangesAsync();
            }
        }
    }
}
