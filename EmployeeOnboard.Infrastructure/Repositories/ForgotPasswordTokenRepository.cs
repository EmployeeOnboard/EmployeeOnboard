using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Infrastructure.Data;
using EmployeeOnboard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboard.Infrastructure.Repositories
{
    public class ForgotPasswordTokenRepository : IForgotPasswordTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public ForgotPasswordTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ForgotPasswordToken> GetByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.ForgotPasswordToken
                .FirstOrDefaultAsync(f => f.EmployeeId == employeeId);
        }

        public async Task AddAsync(ForgotPasswordToken token)
        {
            await _context.ForgotPasswordToken.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ForgotPasswordToken token)
        {
            _context.ForgotPasswordToken.Update(token);
            await _context.SaveChangesAsync();
        }
    }

}
