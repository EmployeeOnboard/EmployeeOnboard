using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboard.Infrastructure.Repositories
{
    public class ResetPasswordRepository : IResetPasswordRepository
    {
        private readonly AppDbContext _context;

        public ResetPasswordRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        } 

        public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;

            // **Check if the new password is the same as the old one**
            if (BCrypt.Net.BCrypt.Verify(newPassword, user.PasswordHash))
            {
                return false; // Prevent updating with the same password
            } 

            // **Hash the new password before saving**
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _context.SaveChangesAsync();
            return true;
        }
    }  
}
