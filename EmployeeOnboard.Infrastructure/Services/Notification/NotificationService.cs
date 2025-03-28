using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Configuration;


namespace EmployeeOnboard.Infrastructure.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _configuration;

        public NotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(EmailRequestDto request)
        {
            try
            {
                var smtpClient = new SmtpClient
                {
                    Host = _configuration["Smtp:Host"],
                    Port = int.Parse(_configuration["Smtp:Port"]),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(
                        _configuration["Smtp:Username"],
                        _configuration["Smtp:Password"]
                    )
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["Smtp:FromEmail"]),
                    Subject = request.Subject,
                    Body = request.Body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(request.To);

                await smtpClient.SendMailAsync(mailMessage);
            }

            catch (SmtpException smtpEx)
            {
                // Log the error or handle specifically for SMTP-related errors
                Console.WriteLine($"SMTP Error: {smtpEx.Message}");
                
            }
            catch (Exception ex)
            {
                // Handle any other general exceptions
                Console.WriteLine($"General Error: {ex.Message}");
                throw;
            }
        }
    }
}