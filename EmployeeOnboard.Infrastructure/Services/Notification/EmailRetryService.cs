
//using EmployeeOnboard.Application.DTOs;
//using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
//using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
//using EmployeeOnboard.Application.Interfaces.Services;
//using Microsoft.Extensions.Logging;

//namespace EmployeeOnboard.Infrastructure.Services.Notification;

//public class EmailRetryService : IEmailRetryService
//{
//    private readonly IEmailLogRepository _emailLogRepository;
//    private readonly INotificationService _notificationService;
//    private readonly ILogger<EmailRetryService> _logger;

//    public EmailRetryService(IEmailLogRepository emailLogRepository, INotificationService notificationService, ILogger<EmailRetryService> logger)
//    {
//        _emailLogRepository = emailLogRepository;
//        _notificationService = notificationService;
//        _logger = logger;
//    }

//    public async Task RetryFailedEmailAsync(CancellationToken cancellationToken)
//    {
//        var failedEmails = await _emailLogRepository.GetFailedEmailLogsAsync(cancellationToken);

//        foreach (var log in failedEmails)
//        {
//            try
//            {
//                // Re-create the email request DTO from the log
//                var emailDto = new EmailRequestDto
//                {
//                    To = log.RecipientEmail,
//                    Subject = log.Subject,
//                    Body = log.Body
//                };

//                await _notificationService.SendEmailAsync(emailDto, cancellationToken);

//                log.IsSuccess = true;
//                log.ErrorMessage = null;

//                _logger.LogInformation("Retried email successfully: {Email}", log.RecipientEmail);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogWarning("Retry failed for {Email}: {Message}", log.RecipientEmail, ex.Message);
//                log.ErrorMessage = ex.Message;
//            }

//            await _emailLogRepository.UpdateAsync(log, cancellationToken);
//        }
//    }
//}
