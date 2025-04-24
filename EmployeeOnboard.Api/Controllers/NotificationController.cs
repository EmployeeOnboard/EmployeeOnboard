using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Infrastructure.Services.Notification;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeOnboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IEmailRetryService _emailRetryService;
        private readonly IEmailLogQueryService _emailLogQueryService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(INotificationService notificationService, IEmailRetryService emailRetryService, IEmailLogQueryService emailLogQueryService, ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _emailRetryService = emailRetryService;
            _emailLogQueryService = emailLogQueryService;
            _logger = logger;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequestDto request)
        {           
            try
            {
                await _notificationService.SendEmailAsync(request);
                return Ok("Email sent successfully!");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Failed to send email. Error: {ex.Message} | StackTrace: {ex.StackTrace}");
            }
        }

        [HttpGet("failed")]
        public async Task<IActionResult> GetFailedEmails()
        {
            try
            {
                var result = await _emailLogQueryService.GetFailedEmailsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving failed emails.");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred while fetching failed emails. Please try again later."
                });
            }
        }

        [HttpPost("retry-selected")]
        public async Task<IActionResult> RetrySelectedEmails([FromBody] List<string> recipientEmails)
        {
            try
            {
                if (recipientEmails == null || !recipientEmails.Any())
                {
                    _logger.LogWarning("Retry email request received without any recipient emails.");
                    return BadRequest(new { success = false, message = "No recipient emails provided." });
                }

                var result = await _emailRetryService.RetrySelectedEmailsAsync(recipientEmails);
                return Ok(new { success = true, message = "Retry operation completed", results = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrying failed emails.");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred while retrying emails. Please try again later."
                });
            }
        }

    }
}
