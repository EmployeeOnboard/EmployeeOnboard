using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;

namespace EmployeeOnboard.Infrastructure.Services.Notification;

public class EmailLogQueryService : IEmailLogQueryService
{
    private readonly IEmailLogRepository _emailLogRepository;
    public EmailLogQueryService(IEmailLogRepository emailLogRepository)
    {
        _emailLogRepository = emailLogRepository;
    }
    public async Task<List<FailedEmailDTO>> GetFailedEmailsAsync()
    {
        var logs = await _emailLogRepository.GetFailedEmailsAsync();

        return logs.Select(log => new FailedEmailDTO
        {
            RecipientEmail = log.RecipientEmail,
            Subject = log.Subject,
            ErrorMessage = log.ErrorMessage,
            CreatedAt = log.SentAt,
            LastRetriedAt = log.LastRetriedAt
        }).ToList();
    }
}
