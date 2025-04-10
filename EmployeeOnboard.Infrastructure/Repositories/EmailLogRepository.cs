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

        public async Task<List<EmailLog>> GetFailedEmailLogsAsync(CancellationToken cancellationToken)
        {
            return await _context.EmailLogs
                .Where(e => !e.IsSuccess)
                .ToListAsync(cancellationToken);
        }

        public async Task LogEmailAsync(EmailLog log, CancellationToken cancellationToken = default)
        {
            await _context.EmailLogs.AddAsync(log, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(EmailLog log, CancellationToken cancellationToken)
        {
            _context.EmailLogs.Update(log);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
