using EmployeeOnboard.Application.DTOs;


namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
    public interface INotificationService
    {
        Task SendEmailAsync(EmailRequestDto request);
    }
}
