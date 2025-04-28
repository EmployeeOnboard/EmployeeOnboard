using EmployeeOnboard.Domain.Entities;

namespace EmployeeOnboard.Application.Interfaces.RepositoryInterfaces
{
    public interface IEmailLogRepository
    {
        Task<EmailLog?> GetByRecipientEmailAsync(string email);
        Task<List<EmailLog>> GetFailedEmailsAsync();
        Task LogEmailAsync(EmailLog log, CancellationToken cancellationToken = default);
        Task UpdateAsync(EmailLog log);

    }
}
