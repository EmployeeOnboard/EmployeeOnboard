using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Infrastructure.Services
{
    public  class EmailService : IEmailService
    {
        public async Task SendResetPasswordEmail(string to, string resetLink)
        {
            var subject = "Reset Your Password";
            var body = $"Click the following link to reset your password: {resetLink}";

            await SendAsync(to, subject, body);
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.example.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("you@example.com", "your-password"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage("you@example.com", to, subject, body);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
 