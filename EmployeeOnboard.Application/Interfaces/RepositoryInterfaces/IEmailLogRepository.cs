using EmployeeOnboard.Domain.Entities;

namespace EmployeeOnboard.Application.Interfaces.RepositoryInterfaces
{
    public interface IEmailLogRepository
    {
        Task LogEmailAsync(EmailLog log, CancellationToken cancellationToken = default);
        Task<List<EmailLog>> GetFailedEmailLogsAsync(CancellationToken cancellationToken);
        Task UpdateAsync(EmailLog log, CancellationToken cancellationToken);

    }
}
