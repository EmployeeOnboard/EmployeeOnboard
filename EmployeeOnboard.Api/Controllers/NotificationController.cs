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

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
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
    }
}
