using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Infrastructure.Data;
using EmployeeOnboard.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmployeeOnboard.Tests.UnitTests.Repositories;

public class EmailLogRepositoryTest
{
    private readonly ApplicationDbContext _context;
    private readonly EmailLogRepository _repository;

    public EmailLogRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new EmailLogRepository(_context);
    }

    [Fact]
    public async Task LogEmailAsync_Should_Add_EmailLog_ToDatabase()
    {
        var log = new EmailLog
        {
            RecipientEmail = "john@example.com",
            Subject = "Test Subject",
            Body = "Test Body",
            IsSuccess = false
        };

        await _repository.LogEmailAsync(log);

        var result = await _context.EmailLogs.FirstOrDefaultAsync(e => e.RecipientEmail == "john@example.com");

        result.Should().NotBeNull();
        result.Subject.Should().Be("Test Subject");
    }
    [Fact]
    public async Task GetByRecipientEmailAsync_Should_Return_CorrectEmailLog()
    {
        var log = new EmailLog { RecipientEmail = "jane@example.com", Subject = "Hello", Body = "This is a test body" };
        await _context.EmailLogs.AddAsync(log);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByRecipientEmailAsync("jane@example.com");

        result.Should().NotBeNull();
        result?.RecipientEmail.Should().Be("jane@example.com");
    }

    [Fact]
    public async Task GetFailedEmailsAsync_Should_Return_OnlyFailedEmails()
    {
        await _context.EmailLogs.AddRangeAsync(
            new EmailLog { RecipientEmail = "a@a.com", Subject = "Hello", Body = "This is a test body", IsSuccess = false },
            new EmailLog { RecipientEmail = "b@b.com", Subject = "Hello", Body = "This is a test body", IsSuccess = true },
            new EmailLog { RecipientEmail = "c@c.com", Subject = "Hello", Body = "This is a test body", IsSuccess = false });

        await _context.SaveChangesAsync();

        var result = await _repository.GetFailedEmailsAsync();

        result.Should().HaveCount(2);
        result.Should().OnlyContain(log => log.IsSuccess == false);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_ExistingEmailLog()
    {
        var log = new EmailLog
        {
            RecipientEmail = "update@test.com",
            Subject = "Initial",
            Body = "This is a test body",
            IsSuccess = false
        };

        await _context.EmailLogs.AddAsync(log);
        await _context.SaveChangesAsync();

        log.IsSuccess = true;
        log.Subject = "Updated";

        await _repository.UpdateAsync(log);

        var updated = await _context.EmailLogs.FirstOrDefaultAsync(e => e.RecipientEmail == "update@test.com");

        updated?.IsSuccess.Should().BeTrue();
        updated?.Subject.Should().Be("Updated");
    }
}

