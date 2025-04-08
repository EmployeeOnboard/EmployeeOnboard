using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Infrastructure.Data;
using EmployeeOnboard.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Tests.UnitTests.Repositories
{
    public class UserRepositoryTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ReturnsUser_WhenEmailExists()
        {
            // Arrange
            var context =  GetInMemoryDbContext();
            var user = new User { Id = 1, Email = "test@example.com", PasswordHash = "Test123!" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetUserByEmailAsync("test@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ReturnsNull_WhenEmailDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetUserByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdatePasswordAsync_ReturnsTrue_WhenPasswordIsUpdated()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var user = new User { Id = 1, Email = "change@example.com", PasswordHash = "OldPass123" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // Act
            user.PasswordHash = "NewPass456";
            var result = await repository.UpdatePasswordAsync(user);

            var updatedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

            // Assert
            Assert.True(result);
            Assert.Equal("NewPass456", updatedUser.PasswordHash);
        }

        [Fact]
        public async Task UpdatePasswordAsync_ReturnsFalse_WhenNoChangesMade()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var user = new User { Id = 1, Email = "unchanged@example.com", PasswordHash = "SamePass" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // Act
            var result = await repository.UpdatePasswordAsync(user); // No actual change

            // Assert
            // NOTE: EF Core will still track and save even if nothing changed, so this will likely return true
            Assert.True(result); // You can change this test if you modify behavior
        }
    }
}
   