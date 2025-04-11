﻿using EmployeeOnboard.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using EmployeeOnboard.Infrastructure.Data;
using EmployeeOnboard.Domain.Entities;


namespace EmployeeOnboard.Infrastructure.Services
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

            user.RefreshToken = null; // Invalidate the refresh token

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
