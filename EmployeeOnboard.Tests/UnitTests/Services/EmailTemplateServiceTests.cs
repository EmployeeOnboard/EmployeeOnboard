using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EmployeeOnboard.Infrastructure.Services.Notification;
using System.Collections.Generic;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using EmployeeOnboard.Domain.Enums;

namespace EmployeeOnboard.Tests.UnitTests.Services
{
    public class EmailTemplateServiceTests
    {
        [Fact]
        public void GetTemplate_ValidKey_ReturnsTemplate()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"EmailTemplates:WelcomeEmail:Subject", "Test Subject"},
                {"EmailTemplates:WelcomeEmail:Body", "Test Body"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var logger = new NullLogger<EmailTemplateService>();

            var service = new EmailTemplateService(configuration, logger);

            // Act
            var templateType = Enum.Parse<EmailTemplateType>("WelcomeEmail");
            var template = service.GetTemplate(templateType);

            // Assert
            Assert.Equal("Test Subject", template.Subject);
            Assert.Equal("Test Body", template.Body);
        }

        [Fact]
        public void GetTemplate_MissingSubject_ReturnsEmptySubject()
        {
            // Arrange
            var configData = new Dictionary<string, string>
    {
        {"EmailTemplates:PartialTemplate:Body", "Only body is present"}
    };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            var logger = new Mock<ILogger<EmailTemplateService>>();
            var service = new EmailTemplateService(configuration, logger.Object);

            // Act
            var templateType = Enum.Parse<EmailTemplateType>("PartialTemplate");
            var result = service.GetTemplate(templateType); // Use the parsed enum here

            // Assert
            Assert.Equal("", result.Subject);
            Assert.Equal("Only body is present", result.Body);
        }

        [Fact]
        public void GetTemplate_MissingBody_ReturnsEmptyBody()
        {
            // Arrange
            var configData = new Dictionary<string, string>
    {
        {"EmailTemplates:PartialTemplate:Subject", "Only subject is present"}
    };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            var logger = new Mock<ILogger<EmailTemplateService>>();
            var service = new EmailTemplateService(configuration, logger.Object);

            // Act
            var templateType = Enum.Parse<EmailTemplateType>("PartialTemplate");
            var result = service.GetTemplate(templateType); // Use the parsed enum here

            // Assert
            Assert.Equal("Only subject is present", result.Subject);
            Assert.Equal("", result.Body);
        }

        [Fact]
        public void GetTemplate_EmptySubjectAndBody_ReturnsEmptyStrings()
        {
            // Arrange
            var configData = new Dictionary<string, string>
    {
        {"EmailTemplates:EmptyTemplate:Subject", ""},
        {"EmailTemplates:EmptyTemplate:Body", ""}
    };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            var logger = new Mock<ILogger<EmailTemplateService>>();
            var service = new EmailTemplateService(configuration, logger.Object);

            // Act
            var templateType = Enum.Parse<EmailTemplateType>("EmptyTemplate");
            var result = service.GetTemplate(templateType); // Use the parsed enum here
            // Assert
            Assert.Equal("", result.Subject);
            Assert.Equal("", result.Body);
        }

        [Fact]
        public void ReplacePlaceholders_ReplacesAllPlaceholdersCorrectly()
        {
            // Arrange
            var service = new EmailTemplateService(new ConfigurationBuilder().Build(), Mock.Of<ILogger<EmailTemplateService>>());

            string subject = "Welcome {UserName}";
            string body = "Hello {UserName}, your role is {Role}.";

            var placeholders = new Dictionary<string, string>
    {
        {"UserName", "Alice"},
        {"Role", "Developer"}
    };

            // Act
            var (finalSubject, finalBody) = service.ReplacePlaceholders(subject, body, placeholders);

            // Assert
            Assert.Equal("Welcome Alice", finalSubject);
            Assert.Equal("Hello Alice, your role is Developer.", finalBody);
        }

        [Fact]
        public void ReplacePlaceholders_NullPlaceholders_ReturnsOriginalTemplate()
        {
            // Arrange
            var service = new EmailTemplateService(new ConfigurationBuilder().Build(), Mock.Of<ILogger<EmailTemplateService>>());

            string subject = "Hello {UserName}";
            string body = "Welcome to the system, {UserName}.";

            // Act
            var (finalSubject, finalBody) = service.ReplacePlaceholders(subject, body, null);

            // Assert
            Assert.Equal(subject, finalSubject);
            Assert.Equal(body, finalBody);
        }

        [Fact]
        public void ReplacePlaceholders_EmptyDictionary_ReturnsOriginalTemplate()
        {
            // Arrange
            var service = new EmailTemplateService(new ConfigurationBuilder().Build(), Mock.Of<ILogger<EmailTemplateService>>());

            string subject = "Welcome {UserName}";
            string body = "Hi {UserName}, your account is ready.";
            var placeholders = new Dictionary<string, string>(); // empty

            // Act
            var (finalSubject, finalBody) = service.ReplacePlaceholders(subject, body, placeholders);

            // Assert
            Assert.Equal(subject, finalSubject);
            Assert.Equal(body, finalBody);
        }

        [Fact]
        public void ReplacePlaceholders_BodyOnlyPlaceholders_ReplacesCorrectly()
        {
            // Arrange
            var service = new EmailTemplateService(new ConfigurationBuilder().Build(), Mock.Of<ILogger<EmailTemplateService>>());

            string subject = "Welcome Email";
            string body = "Hello {UserName}, welcome!";
            var placeholders = new Dictionary<string, string>
    {
        { "UserName", "Jane" }
    };

            // Act
            var (finalSubject, finalBody) = service.ReplacePlaceholders(subject, body, placeholders);

            // Assert
            Assert.Equal("Welcome Email", finalSubject);
            Assert.Equal("Hello Jane, welcome!", finalBody);
        }

        [Fact]
        public void ReplacePlaceholders_SubjectOnlyPlaceholders_ReplacesCorrectly()
        {
            // Arrange
            var service = new EmailTemplateService(new ConfigurationBuilder().Build(), Mock.Of<ILogger<EmailTemplateService>>());

            string subject = "Hello {UserName}";
            string body = "No placeholders here.";
            var placeholders = new Dictionary<string, string>
    {
        { "UserName", "Jane" }
    };

            // Act
            var (finalSubject, finalBody) = service.ReplacePlaceholders(subject, body, placeholders);

            // Assert
            Assert.Equal("Hello Jane", finalSubject);
            Assert.Equal(body, finalBody);
        }

        [Fact]
        public void ReplacePlaceholders_MultiplePlaceholders_ReplacesAllCorrectly()
        {
            // Arrange
            var service = new EmailTemplateService(new ConfigurationBuilder().Build(), Mock.Of<ILogger<EmailTemplateService>>());

            string subject = "Hello {UserName}";
            string body = "Hi {UserName}, your role is {UserRole}.";
            var placeholders = new Dictionary<string, string>
    {
        { "UserName", "John" },
        { "UserRole", "Developer" }
    };

            // Act
            var (finalSubject, finalBody) = service.ReplacePlaceholders(subject, body, placeholders);

            // Assert
            Assert.Equal("Hello John", finalSubject);
            Assert.Equal("Hi John, your role is Developer.", finalBody);
        }

        [Fact]
        public void ReplacePlaceholders_UnusedPlaceholder_DoesNothing()
        {
            // Arrange
            var service = new EmailTemplateService(new ConfigurationBuilder().Build(), Mock.Of<ILogger<EmailTemplateService>>());

            string subject = "Hi there";
            string body = "Welcome to the platform!";
            var placeholders = new Dictionary<string, string>
    {
        { "UnusedKey", "Value" }
    };

            // Act
            var (finalSubject, finalBody) = service.ReplacePlaceholders(subject, body, placeholders);

            // Assert
            Assert.Equal(subject, finalSubject);
            Assert.Equal(body, finalBody);
        }


    }
}
