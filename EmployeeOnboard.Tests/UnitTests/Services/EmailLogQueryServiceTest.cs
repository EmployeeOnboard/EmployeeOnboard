
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Infrastructure.Services.Notification;
using FluentAssertions;
using Moq;

namespace EmployeeOnboard.Tests.UnitTests.Services
{
    public class EmailLogQueryServiceTest
    {
        private readonly Mock<IEmailLogRepository> _emailLogRepoMock;
        private readonly EmailLogQueryService _queryService;

        public EmailLogQueryServiceTest()
        {
            _emailLogRepoMock = new Mock<IEmailLogRepository>();
            _queryService = new EmailLogQueryService(_emailLogRepoMock.Object);
        }
        [Fact]
        public async Task GetFailedEmailsAsync_ShouldReturnMappedFailedEmailDTOs()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var mockLogs = new List<EmailLog>
        {
            new EmailLog
            {
                RecipientEmail = "fail1@example.com",
                Subject = "Subject 1",
                ErrorMessage = "SMTP timeout",
                SentAt = now.AddMinutes(-20),
                LastRetriedAt = now.AddMinutes(-5)
            },
            new EmailLog
            {
                RecipientEmail = "fail2@example.com",
                Subject = "Subject 2",
                ErrorMessage = "Connection refused",
                SentAt = now.AddMinutes(-10),
                LastRetriedAt = now.AddMinutes(-2)
            }
        };

            _emailLogRepoMock.Setup(repo => repo.GetFailedEmailsAsync()).ReturnsAsync(mockLogs);

            // Act
            var result = await _queryService.GetFailedEmailsAsync();

            // Assert
            result.Should().HaveCount(2);
            result[0].RecipientEmail.Should().Be("fail1@example.com");
            result[0].ErrorMessage.Should().Be("SMTP timeout");
            result[1].LastRetriedAt.Should().HaveValue();
        }

    }
}
