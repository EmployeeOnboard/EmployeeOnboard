using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Domain.Enums;
using EmployeeOnboard.Domain.Models;
using EmployeeOnboard.Infrastructure.Services.Notification;
using EmployeeOnboard.Infrastructure.Services.PasswordManagementService;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace EmployeeOnboard.Tests.Services.PasswordManagement
{
    public class ForgotPasswordServiceTest
    {
        private readonly Mock<IEmployeeRepository> _employeeRepoMock = new();
        private readonly Mock<INotificationService> _notificationServiceMock = new();
        private readonly Mock<IConfiguration> _configMock = new();
        private readonly Mock<EmailTemplateService> _emailTemplateServiceMock = new();
        private readonly Mock<IForgotPasswordTokenRepository> _tokenRepoMock = new();
        private readonly ForgotPasswordService _service;

        public ForgotPasswordServiceTest()
        {
            _emailTemplateServiceMock.Setup(x => x.GetTemplate(EmailTemplateType.ForgotPassword))
                .Returns(new EmailTemplateModel
                {
                    Subject = "Reset your password",
                    Body = "Hi {{UserName}}, click here: {{ResetLink}}. It expires in {{ExpiryTime}}."
                });

            _emailTemplateServiceMock.Setup(x => x.ReplacePlaceholders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .Returns(("Reset your password", "Processed Email Body"));

            _configMock.Setup(x => x["AppSettings:FrontendBaseUrl"]).Returns("https://frontend.com");

            _service = new ForgotPasswordService(
                _employeeRepoMock.Object,
                _notificationServiceMock.Object,
                _configMock.Object,
                _emailTemplateServiceMock.Object,
                _tokenRepoMock.Object
            );
        }

        [Fact]
        public async Task ForgotPasswordAsync_ShouldCreateTokenAndSendEmail_WhenUserExists_AndNoTokenExists()
        {
            // Arrange
            var email = "test@example.com";
            var user = new Employee { Id = Guid.NewGuid(), Email = email, FirstName = "Test", LastName = "User" };

            _employeeRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            _tokenRepoMock.Setup(r => r.GetByEmployeeIdAsync(user.Id)).ReturnsAsync((ForgotPasswordToken)null);

            // Act
            await _service.ForgotPasswordAsync(email);

            // Assert
            _tokenRepoMock.Verify(r => r.AddAsync(It.IsAny<ForgotPasswordToken>()), Times.Once);
            _notificationServiceMock.Verify(r => r.SendEmailAsync(It.IsAny<EmailRequestDto>()), Times.Once);
        }

        [Fact]
        public async Task ForgotPasswordAsync_ShouldUpdateToken_WhenTokenExists()
        {
            // Arrange
            var email = "test@example.com";
            var userId = Guid.NewGuid();
            var user = new Employee { Id = userId, Email = email, FirstName = "Test", LastName = "User" };
            var existingToken = new ForgotPasswordToken { EmployeeId = userId };

            _employeeRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            _tokenRepoMock.Setup(r => r.GetByEmployeeIdAsync(userId)).ReturnsAsync(existingToken);

            // Act
            await _service.ForgotPasswordAsync(email);

            // Assert
            _tokenRepoMock.Verify(r => r.UpdateAsync(It.Is<ForgotPasswordToken>(t =>
                t.EmployeeId == userId &&
                !string.IsNullOrEmpty(t.PasswordResetToken) &&
                t.PasswordResetTokenExpiry > DateTime.UtcNow
            )), Times.Once);
        }

        [Fact]
        public async Task ForgotPasswordAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var email = "notfound@example.com";
            _employeeRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((Employee)null);

            // Act
            Func<Task> act = () => _service.ForgotPasswordAsync(email);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("User with that email does not exist.");
        }
    }
}
