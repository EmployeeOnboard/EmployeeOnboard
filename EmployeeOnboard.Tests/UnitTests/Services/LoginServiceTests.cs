using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using EmployeeOnboard.Infrastructure.Services;
using EmployeeOnboard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Application.DTOs;


public class AuthServiceTests
{
    private readonly LoginService _authService;
    private readonly ApplicationDbContext _dbContext;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<LoginService>> _mockLogger;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<LoginService>>();

        _mockConfiguration.Setup(c => c["Jwt:Secret"]).Returns("ablGWCOVfKp/HCvDsJTR1j+7Yw4bZ6enLyFdHuKXWHM=");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("https://localhost:7106");

        _authService = new LoginService(_dbContext, _mockConfiguration.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var testUser = new Users
        {
            Id = Guid.NewGuid(),
            Name = "John",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct_password")
            //PasswordHash = "hashed_password"
        };

        await _dbContext.Users.AddAsync(testUser);
        await _dbContext.SaveChangesAsync();

        var loginDto = new LoginDTO { Email = "test@example.com", Password = "correct_password" };


        // Act
        var result = await _authService.LoginAsync(loginDto);

        Console.WriteLine($"Success: {result.Success}, Token: {result.Token}");

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Token);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDTO { Email = "wrong@example.com", Password = "wrong_password" };

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Token);
    }
}
