

using EmployeeOnboard.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using EmployeeOnboard.Infrastructure.Data;
using EmployeeOnboard.Domain.Entities;


namespace EmployeeOnboard.Infrastructure.Services.Employees
{
    public class LogoutService : ILogoutService
    {
        private readonly ApplicationDbContext _context;

        public LogoutService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> LogoutAsync(Guid userId)
        {
            var user = await _context.Employees.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return false;

            // Find the refresh token associated with the user
            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.EmployeeId == user.Id)
                .ToListAsync();

            if (refreshTokens.Any())
            {
                _context.RefreshTokens.RemoveRange(refreshTokens);
                await _context.SaveChangesAsync();
            }


            return true;
        }

    }
}
