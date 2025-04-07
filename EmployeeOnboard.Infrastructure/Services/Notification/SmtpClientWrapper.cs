using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using EmployeeOnboard.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

public class SmtpClientWrapper : ISmtpClientWrapper
{
    private readonly SmtpClient _smtpClient;


    public SmtpClientWrapper(string host, int port, string userName, string password, bool enableSsl)
    {
        // Initialize the SmtpClient with parameters
        _smtpClient = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(userName, password),
            EnableSsl = enableSsl,
            UseDefaultCredentials = false,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };
    }

    public Task SendMailAsync(MailMessage mailMessage)
    {
        return _smtpClient.SendMailAsync(mailMessage);
    }
}
