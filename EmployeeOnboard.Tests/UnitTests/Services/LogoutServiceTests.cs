
using Microsoft.EntityFrameworkCore;
using Xunit;
using EmployeeOnboard.Infrastructure.Services;
using EmployeeOnboard.Infrastructure.Persistence;
using EmployeeOnboard.Domain.Entities;

public class LogoutServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly LogoutService _logoutService;

    public LogoutServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())  // Unique database for each test
            .Options;

        _context = new ApplicationDbContext(options);
        _logoutService = new LogoutService(_context);
    }

    [Fact]
    public async Task LogoutAsync_UserExists_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new Users { Id = userId, RefreshToken = "some_token" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _logoutService.LogoutAsync(userId);

        // Assert
        Assert.True(result);

        var updatedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        Assert.Null(updatedUser.RefreshToken);  // Ensure the token was cleared
    }

    [Fact]
    public async Task LogoutAsync_UserDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid(); // Non-existing user ID

        // Act
        var result = await _logoutService.LogoutAsync(userId);

        // Assert
        Assert.False(result);
    }
}
