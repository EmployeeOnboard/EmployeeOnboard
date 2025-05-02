//using Xunit;
//using Moq;
//using FluentAssertions;
//using EmployeeOnboard.Application.DTOs;
//using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
//using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
//using EmployeeOnboard.Domain.Entities;
//using EmployeeOnboard.Infrastructure.Services.Notification;
//using Microsoft.Extensions.Logging;

//namespace EmployeeOnboard.Tests.UnitTests.Services;

//public class EmailRetryServiceTest
//{
//    private readonly Mock<IEmailLogRepository> _emailLogRepoMock = new();
//    private readonly Mock<IEmployeeRepository> _employeeRepoMock = new();
//    private readonly Mock<INotificationService> _notificationServiceMock = new();
//    private readonly Mock<ILogger<EmailRetryService>> _loggerMock = new();

//    private readonly EmailRetryService _retryService;

//    public EmailRetryServiceTest()
//    {
//        _retryService = new EmailRetryService(
//            _emailLogRepoMock.Object,
//            _notificationServiceMock.Object,
//            _employeeRepoMock.Object,
//            _loggerMock.Object);
//    }

//    [Fact]
//    public async Task RetrySelectedEmailsAsync_ShouldSkip_WhenEmailNotFoundOrAlreadySucceeded()
//    {
//        _emailLogRepoMock.Setup(r => r.GetByRecipientEmailAsync("test@example.com"))
//            .ReturnsAsync((EmailLog?)null);

//        var result = await _retryService.RetrySelectedEmailsAsync(new List<string> { "test@example.com" });

//        result.Should().ContainSingle()
//            .Which.Should().Contain("Skipped: test@example.com");
//    }

//    [Fact]
//    public async Task RetrySelectedEmailsAsync_ShouldReturnError_WhenEmployeeNotFound()
//    {
//        var log = new EmailLog { RecipientEmail = "fail@example.com", Subject = "Hello", Body = "This is a test body", IsSuccess = false };
//        _emailLogRepoMock.Setup(r => r.GetByRecipientEmailAsync("fail@example.com")).ReturnsAsync(log);
//        _employeeRepoMock.Setup(r => r.GetByEmailAsync("fail@example.com")).ReturnsAsync((Employee?)null);

//        var result = await _retryService.RetrySelectedEmailsAsync(new List<string> { "fail@example.com" });

//        result.Should().Contain("❌ No employee found for fail@example.com");
//    }

//    [Fact]
//    public async Task RetrySelectedEmailsAsync_ShouldResendEmail_WhenConditionsAreValid()
//    {
//        var log = new EmailLog { RecipientEmail = "john@example.com", Subject = "Hello", Body = "This is a test body", IsSuccess = false };
//        var employee = new Employee
//        {
//            Email = "john@example.com",
//            FirstName = "John",
//            LastName = "Doe",
//            EmployeeNumber = "EMP001"
//        };

//        _emailLogRepoMock.Setup(r => r.GetByRecipientEmailAsync("john@example.com")).ReturnsAsync(log);
//        _employeeRepoMock.Setup(r => r.GetByEmailAsync("john@example.com")).ReturnsAsync(employee);
//        _notificationServiceMock.Setup(n => n.SendEmailAsync(It.IsAny<EmailRequestDto>())).Returns(Task.CompletedTask);

//        var result = await _retryService.RetrySelectedEmailsAsync(new List<string> { "john@example.com" });

//        result.Should().Contain("✅ Email resent to john@example.com");
//        log.IsSuccess.Should().BeTrue();
//    }

//    [Fact]
//    public async Task RetrySelectedEmailsAsync_ShouldHandleExceptions_WhenSendingFails()
//    {
//        var log = new EmailLog { RecipientEmail = "error@example.com", Subject = "Hello", Body = "This is a test body", IsSuccess = false };
//        var employee = new Employee
//        {
//            Email = "error@example.com",
//            FirstName = "Jane",
//            LastName = "Doe",
//            EmployeeNumber = "EMP002"
//        };

//        _emailLogRepoMock.Setup(r => r.GetByRecipientEmailAsync("error@example.com")).ReturnsAsync(log);
//        _employeeRepoMock.Setup(r => r.GetByEmailAsync("error@example.com")).ReturnsAsync(employee);
//        _notificationServiceMock
//            .Setup(n => n.SendEmailAsync(It.IsAny<EmailRequestDto>()))
//            .ThrowsAsync(new Exception("SMTP error"));

//        var result = await _retryService.RetrySelectedEmailsAsync(new List<string> { "error@example.com" });

//        result.Should().Contain(r => r.Contains("❌ Retry failed for error@example.com"));
//        log.IsSuccess.Should().BeFalse();
//    }
//}
