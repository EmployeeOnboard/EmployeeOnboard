using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboard.Infrastructure.Repositories
{
    public class EmailLogRepository : IEmailLogRepository
    {
        private readonly ApplicationDbContext _context;

        public EmailLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EmailLog?> GetByRecipientEmailAsync(string email)
        {
            return await _context.EmailLogs.FirstOrDefaultAsync(e => e.RecipientEmail == email);
        }

        public async Task<List<EmailLog>> GetFailedEmailsAsync()
        {
            return await _context.EmailLogs
                .Where(e => !e.IsSuccess)
                .OrderByDescending(e => e.SentAt)
                .ToListAsync();
        }

        public async Task LogEmailAsync(EmailLog log, CancellationToken cancellationToken = default)
        {
            await _context.EmailLogs.AddAsync(log, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(EmailLog log)
        {
            _context.EmailLogs.Update(log);
            await _context.SaveChangesAsync();
        }
    }
}
