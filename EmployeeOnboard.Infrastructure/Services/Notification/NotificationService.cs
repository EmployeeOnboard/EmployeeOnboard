using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;
using EmployeeOnboard.Application.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EmployeeOnboard.Domain.Models;
using Azure.Core;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Domain.Enums;


namespace EmployeeOnboard.Infrastructure.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationService> _logger;
        private readonly string _templatesPath;
        private readonly EmailTemplateService _emailTemplateService;
        private readonly ISmtpClientWrapper _smtpClientWrapper;


        public NotificationService(IConfiguration configuration, ISmtpClientWrapper smtpClientWrapper, EmailTemplateService emailTemplateService, ILogger<NotificationService> logger)
        {
            _smtpClientWrapper = smtpClientWrapper;
            _emailTemplateService = emailTemplateService;
            _logger = logger;
            _configuration = configuration;

        }


        public async Task SendEmailAsync(EmailRequestDto request)
        {
            try
            {
                // Get the email template
                var template = _emailTemplateService.GetTemplate(EmailTemplateType.WelcomeEmail); // ✅
                var (subject, body) = _emailTemplateService.ReplacePlaceholders(template.Subject, template.Body, request.Placeholders);


                var smtpSettings = _configuration.GetSection("Smtp");



                // Configure the SMTP client
                using (var client = new SmtpClient(smtpSettings["Host"], int.Parse(smtpSettings["Port"])))
                {
                    client.Credentials = new NetworkCredential(smtpSettings["UserName"], smtpSettings["Password"]);
                    client.EnableSsl = bool.Parse(smtpSettings["EnableSsl"]); // Ensure it's true for TLS/SSL
                    client.UseDefaultCredentials = false; // Ensure it's false
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;


                    // Create a new mail message
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("employeeonboard6@gmail.com"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(request.To); //Adds the recipients email address


                    // Send Email
                    try
                    {
                        await client.SendMailAsync(mailMessage);
                        _logger.LogInformation($"Email sent successfully to {request.To}");
                    }
                    catch (SmtpException ex)
                    {
                        _logger.LogError($"SMTP Exception: {ex.StatusCode} - {ex.Message}");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"General Exception: {ex.Message}");
                        throw;
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email. Error: {ex.Message} | StackTrace: {ex.StackTrace}");
                throw new Exception($"Failed to send email: {ex.Message}", ex);
            }
        }
    }
}


