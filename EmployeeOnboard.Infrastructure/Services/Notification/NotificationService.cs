using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EmployeeOnboard.Domain.Models;
using Azure.Core;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;


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
                var template = _emailTemplateService.GetTemplate(request.TemplateKey); // Await the GetTemplate method
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


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Net.Mail;
//using System.Text;
//using System.Text.Encodings.Web;
//using System.Text.Json;
//using System.Threading.Tasks;
//using EmployeeOnboard.Application.DTOs;
//using EmployeeOnboard.Application.Interfaces.Services;
//using MediatR;
//using Microsoft.Extensions.Configuration;
//using EmployeeOnboard.Infrastructure.Models;



//namespace EmployeeOnboard.Infrastructure.Services.Notification
//{
//    public class NotificationService : INotificationService
//    {
//        private readonly IConfiguration _configuration;

//        private readonly string _templatesPath;

//        public NotificationService(IConfiguration configuration)
//        {
//            _configuration = configuration;

//            _templatesPath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "EmailTemplates.json");

//        }

//        public async Task SendEmailAsync(EmailRequestDto request)
//        {
//            try
//            {

//                if (!File.Exists(_templatesPath))
//                {
//                    throw new FileNotFoundException($"Email template file not found at {_templatesPath}");
//                }

//                string jsonContent = await File.ReadAllTextAsync(_templatesPath);

//                var templates = JsonSerializer.Deserialize<Dictionary<string, EmailTemplate>>(jsonContent);

//                // ✅ Check if the template exists
//                if (!templates.ContainsKey(request.TemplateKey))
//                {
//                    throw new KeyNotFoundException($"Email template '{request.TemplateKey}' not found.");
//                }

//                var template = templates[request.TemplateKey];

//                // Retrieve the correct template path based on the TemplateKey
//                if (!templates.TryGetValue(request.TemplateKey, out EmailTemplate? templatePath))
//                {
//                    throw new KeyNotFoundException($"No email template found for key: {request.TemplateKey}");
//                }

//                // ✅ Replace placeholders dynamically
//                string emailBody = template.Body;
//                string emailSubject = template.Subject;

//                foreach (var placeholder in request.Placeholders)
//                {
//                    emailBody = emailBody.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
//                    emailSubject = emailSubject.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
//                }

//                var smtpClient = new SmtpClient
//                {
//                    Host = _configuration["Smtp:Host"],
//                    Port = int.Parse(_configuration["Smtp:Port"]),
//                    EnableSsl = true,
//                    DeliveryMethod = SmtpDeliveryMethod.Network,
//                    UseDefaultCredentials = false,
//                    Credentials = new NetworkCredential(
//                        _configuration["Smtp:Username"],
//                        _configuration["Smtp:Password"]
//                    )
//                };

//                var mailMessage = new MailMessage
//                {
//                    From = new MailAddress(_configuration["Smtp:FromEmail"]),
//                    Subject = emailSubject,
//                    Body = emailBody,
//                    IsBodyHtml = true
//                };

//                mailMessage.To.Add(request.To);

//                await smtpClient.SendMailAsync(mailMessage);
//            }

//            catch (Exception ex)
//            {
//                throw new Exception($"Failed to send email. Error: {ex.Message} | StackTrace: {ex.StackTrace}");
//            }
//        }
//    }
//}