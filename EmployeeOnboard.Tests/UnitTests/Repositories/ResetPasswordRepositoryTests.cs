using Castle.Core.Logging;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Infrastructure.Data;
using EmployeeOnboard.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Tests.UnitTests.Repositories
{

        public class ResetPasswordRepositoryTests
        {
            private AppDbContext GetInMemoryDbContext()
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique DB for isolation
                    .Options;

                return new AppDbContext(options);
            }

            [Fact]
            public async Task UpdatePasswordAsync_UserNotFound_ReturnsFalse()
            {
                // Arrange
                var context = GetInMemoryDbContext();
                var repository = new ResetPasswordRepository(context);

                // Act
                var result = await repository.UpdatePasswordAsync("nonexistent@example.com", "newPass");

                // Assert
                Assert.False(result);
            }

            [Fact]
            public async Task UpdatePasswordAsync_NewPasswordMatchesOld_ReturnsFalse()
            {
                // Arrange
                var context = GetInMemoryDbContext();
                var user = new User
                {
                    Email = "test@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
                };
                context.Users.Add(user);
                await context.SaveChangesAsync();

                var repository = new ResetPasswordRepository(context);

                // Act
                var result = await repository.UpdatePasswordAsync("test@example.com", "password123");

                // Assert
                Assert.False(result);
            }

            [Fact]
            public async Task UpdatePasswordAsync_ValidPasswordUpdate_ReturnsTrue()
            {
                // Arrange
                var context = GetInMemoryDbContext();
                var oldPassword = "oldPassword";
                var newPassword = "newSecurePassword";

                var user = new User
                {
                    Email = "user@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(oldPassword)
                };
                context.Users.Add(user);
                await context.SaveChangesAsync();

                var repository = new ResetPasswordRepository(context);

                // Act
                var result = await repository.UpdatePasswordAsync("user@example.com", newPassword);

                // Assert
                Assert.True(result);

                var updatedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "user@example.com");
                Assert.NotNull(updatedUser);
                Assert.True(BCrypt.Net.BCrypt.Verify(newPassword, updatedUser.PasswordHash));
            }
        }

}
