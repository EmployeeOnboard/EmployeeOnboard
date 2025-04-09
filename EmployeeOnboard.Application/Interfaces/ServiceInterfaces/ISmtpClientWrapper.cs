using System.Net.Mail;


namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
    public interface ISmtpClientWrapper
    {
        Task SendMailAsync(MailMessage message);
    }
}

