using EmployeeOnboard.Application.DTOs;


namespace EmployeeOnboard.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task SendEmailAsync(EmailRequestDto request);
    }
}
