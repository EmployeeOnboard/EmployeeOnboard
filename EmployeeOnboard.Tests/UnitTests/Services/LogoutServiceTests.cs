﻿
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Infrastructure.Data;
using EmployeeOnboard.Infrastructure.Services.Employees;
using System.Security.Claims;
using System.Collections.Generic;

public class LogoutServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly LogoutService _logoutService;
    private readonly Mock<ILogger<LogoutService>> _mockLogger;

    public LogoutServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<LogoutService>>();
        _logoutService = new LogoutService(_context, _mockLogger.Object);
    }

    [Fact]
    public async Task LogoutAsync_UserExists_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new Employee
        {
            Id = userId,
            Email = "test@example.com",
            Password = "hashed_password"
        };

        var refreshToken = new RefreshToken
        {
            Token = "some_token",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            EmployeeId = userId
        };

        _context.Employees.Add(user);
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        var claims = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }));

        // Act
        var (success, message) = await _logoutService.LogoutAsync(claims);

        // Assert
        Assert.True(success);
        Assert.Equal("Logged out successfully", message);
    }

    [Fact]
    public async Task LogoutAsync_InvalidUser_ReturnsFailure()
    {
        // Arrange
        var claims = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        }));

        // Act
        var (success, message) = await _logoutService.LogoutAsync(claims);

        // Assert
        Assert.False(success);
    }
}
