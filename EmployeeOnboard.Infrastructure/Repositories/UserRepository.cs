using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EmployeeOnboard.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        // Constructor to inject the AppDbContext
        public UserRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));    
        }

        // Implementation of GetUserByEmailAsync method
        public async Task<User> GetUserByEmailAsync(string email)
        {
            // Query the Users table and return the first match or null if not found
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email); // Using EF Core's FirstOrDefaultAsync for async database call
        }
        public async Task<bool> UpdatePasswordAsync(User user)
        {
            _context.Users.Update(user); // Mark user entity as updated
            return await _context.SaveChangesAsync() > 0; // Save changes to DB
        } 
    }
} 
    