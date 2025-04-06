//using System;
//using System.Collections.Generic;
//using System.Net.Mail;
//using System.Threading.Tasks;
//using EmployeeOnboard.Application.DTOs;
//using EmployeeOnboard.Application.Interfaces.Services;
//using EmployeeOnboard.Infrastructure.Models;
//using EmployeeOnboard.Infrastructure.Services.Notification;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Xunit;

//namespace EmployeeOnboard.Tests.UnitTests.Services
//{
//        public class NotificationServiceTests
//        {
//            private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
//            private readonly Mock<ISmtpClientWrapper> _smtpClientWrapperMock;
//            private readonly Mock<ILogger<NotificationService>> _loggerMock;
//            private readonly NotificationService _notificationService;

//            public NotificationServiceTests()
//            {
//                _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
//                _smtpClientWrapperMock = new Mock<ISmtpClientWrapper>();
//                _loggerMock = new Mock<ILogger<NotificationService>>();

//                // Set up the notification service
//                _notificationService = new NotificationService(
//                    Mock.Of<IConfiguration>(),
//                    _loggerMock.Object,
//                    _emailTemplateServiceMock.Object,
//                    _smtpClientWrapperMock.Object
//                );
//            }

//            [Fact]
//            public async Task SendEmailSuccessfully()
//            {
//                // Arrange
//                var emailRequest = new EmailRequestDto
//                {
//                    To = "test@example.com",
//                    TemplateKey = "PasswordReset",
//                    Placeholders = new Dictionary<string, string> { { "UserName", "John" }, { "ResetLink", "http://resetlink.com" } }
//                };

//                var template = new EmailTemplate { Subject = "Reset Password", Body = "<html><body>Reset link</body></html>" };
//                _emailTemplateServiceMock.Setup(service => service.GetTemplate(It.IsAny<string>())).Returns(template);
//                _emailTemplateServiceMock.Setup(service => service.ReplacePlaceholders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
//                    .Returns((string subject, string body, Dictionary<string, string> placeholders) => (subject, body));

//                // Act
//                await _notificationService.SendEmailAsync(emailRequest);

//                // Assert
//                _smtpClientWrapperMock.Verify(client => client.SendMailAsync(It.IsAny<MailMessage>()), Times.Once);
//                _loggerMock.Verify(logger => logger.LogInformation(It.Is<string>(s => s.Contains("Email sent successfully"))), Times.Once);
//            }
//        }
//    }
//}

