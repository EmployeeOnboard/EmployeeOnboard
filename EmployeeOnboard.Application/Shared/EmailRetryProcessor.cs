using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeOnboard.Application.Shared
{
    public class EmailRetryProcessor : IEmailRetryProcessor
    {
        private readonly IEmailLogRepository _emailLogRepository;
        private readonly INotificationService _notificationService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmailRetryProcessor> _logger;

        public EmailRetryProcessor(IEmailLogRepository emailLogRepository, INotificationService notificationService, IEmployeeRepository employeeRepository, ILogger<EmailRetryProcessor> logger)
        {
            _emailLogRepository = emailLogRepository;
            _notificationService = notificationService;
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task<string> RetryEmailAsync(string recipientEmail)
        {
            var log = await _emailLogRepository.GetByRecipientEmailAsync(recipientEmail);

            if (log == null || log.IsSuccess)
                return $"ℹ️ Skipped: {recipientEmail} not found or already succeeded.";

            try
            {
                var employee = await _employeeRepository.GetByEmailAsync(recipientEmail);
                if (employee == null)
                    return $"No employee found for {recipientEmail}";

                var emailDto = new EmailRequestDto
                {
                    To = recipientEmail,
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

                return $"Email resent to {recipientEmail}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Retry failed for {Email}", recipientEmail);
                return $"Retry failed for {recipientEmail}: {ex.Message}";
            }
        }

    }

}


