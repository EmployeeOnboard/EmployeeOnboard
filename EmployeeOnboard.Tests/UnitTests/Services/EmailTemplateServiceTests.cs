using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EmployeeOnboard.Infrastructure.Services.Notification;
using EmployeeOnboard.Infrastructure.Models;
using System.Collections.Generic;


namespace EmployeeOnboard.Tests.UnitTests.Services
{
    public class EmailTemplateServiceTests
    {

    [Fact]
    public void GetTemplate_ValidKey_ReturnsTemplate()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();
        var logger = new Mock<ILogger<EmailTemplateService>>();

        mockSection.Setup(s => s.Exists()).Returns(true);
        mockSection.Setup(s => s["Subject"]).Returns("Test Subject");
        mockSection.Setup(s => s["Body"]).Returns("Test Body");

        mockConfig.Setup(c => c.GetSection("EmailTemplates:Welcome")).Returns(mockSection.Object);

        var service = new EmailTemplateService(mockConfig.Object, logger.Object);

        // Act
        var template = service.GetTemplate("Welcome");

        // Assert
        Assert.Equal("Test Subject", template.Subject);
        Assert.Equal("Test Body", template.Body);
    }

    [Fact]
    public void GetTemplate_InvalidKey_ThrowsKeyNotFoundException()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();
        var logger = new Mock<ILogger<EmailTemplateService>>();

        mockSection.Setup(s => s.Exists()).Returns(false);
        mockConfig.Setup(c => c.GetSection("EmailTemplates:InvalidKey")).Returns(mockSection.Object);

        var service = new EmailTemplateService(mockConfig.Object, logger.Object);

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => service.GetTemplate("InvalidKey"));
    }

    [Fact]
    public void ReplacePlaceholders_ReplacesAllPlaceholders_ReturnsReplacedTemplate()
    {
        // Arrange
        var service = new EmailTemplateService(new Mock<IConfiguration>().Object, new Mock<ILogger<EmailTemplateService>>().Object);
        var subject = "Hello {UserName}";
        var body = "Welcome to the system, {UserName}.";
        var placeholders = new Dictionary<string, string>
    {
        { "UserName", "John Doe" }
    };

        // Act
        var result = service.ReplacePlaceholders(subject, body, placeholders);

        // Assert
        Assert.Equal("Hello John Doe", result.Subject);
        Assert.Equal("Welcome to the system, John Doe.", result.Body);
    }

    [Fact]
    public void ReplacePlaceholders_NoPlaceholders_ReturnsOriginalTemplate()
    {
        // Arrange
        var service = new EmailTemplateService(new Mock<IConfiguration>().Object, new Mock<ILogger<EmailTemplateService>>().Object);
        var subject = "Hello {UserName}";
        var body = "Welcome to the system, {UserName}.";

        // Act
        var result = service.ReplacePlaceholders(subject, body, null);

        // Assert
        Assert.Equal(subject, result.Subject);
        Assert.Equal(body, result.Body);
    }

}
