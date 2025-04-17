using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EmployeeOnboard.Infrastructure.Services.Notification;

public class EmailRetryService : IEmailRetryService
{
    private readonly IEmailLogRepository _emailLogRepository;
    private readonly INotificationService _notificationService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<EmailRetryService> _logger;

    public EmailRetryService(IEmailLogRepository emailLogRepository, INotificationService notificationService, IEmployeeRepository employeeRepository, ILogger<EmailRetryService> logger)
    {
        _emailLogRepository = emailLogRepository;
        _notificationService = notificationService;
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task<List<string>> RetrySelectedEmailsAsync(List<string> recipientEmails)
    {
        var results = new List<string>();

        foreach (var email in recipientEmails)
        {
            var log = await _emailLogRepository.GetByRecipientEmailAsync(email);

            if (log == null || log.IsSuccess)
            {
                results.Add($"ℹ️ Skipped: {email} not found or already succeeded.");
                continue;
            }

            try
            {
                var employee = await _employeeRepository.GetByEmailAsync(log.RecipientEmail);
                if (employee == null)
                {
                    results.Add($"❌ No employee found for {log.RecipientEmail}");
                    continue;
                }

                var emailDto = new EmailRequestDto
                {
                    To = log.RecipientEmail,
                    TemplateKey = log.TemplateKey ?? "WelcomeEmail",
                    Placeholders = new Dictionary<string, string>
                        {
                            { "FullName", $"{employee.FirstName} {employee.LastName}" },
                            { "EmployeeNumber", employee.EmployeeNumber },
                            { "Password", "********" }
                        }
                };

                await _notificationService.SendEmailAsync(emailDto);

                log.IsSuccess = true;
                log.ErrorMessage = null;
                log.LastRetriedAt = DateTime.UtcNow;

                await _emailLogRepository.UpdateAsync(log);

                results.Add($"✅ Email resent to {log.RecipientEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Retry failed for email to {Email}", log.RecipientEmail);
                results.Add($"❌ Retry failed for {log.RecipientEmail}: {ex.Message}");
            }
        }

        return results;
    }
}
