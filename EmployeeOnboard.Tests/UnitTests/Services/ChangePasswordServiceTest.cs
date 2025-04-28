using EmployeeOnboard.Application.DTOs.PasswordManagementDTO;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Infrastructure.Services.PasswordManagementService;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Tests.UnitTests.Services
{
    public class ChangePasswordServiceTest;

namespace EmployeeOnboard.Tests.Services.PasswordManagement
    {
        public class ChangePasswordServiceTests
        {
            private readonly Mock<IEmployeeRepository> _mockRepo;
            private readonly ChangePasswordService _service;

            public ChangePasswordServiceTests()
            {
                _mockRepo = new Mock<IEmployeeRepository>();
                _service = new ChangePasswordService(_mockRepo.Object);
            }

            [Fact]
            public async Task ChangePasswordAsync_ShouldUpdatePassword_WhenValid()
            {
                // Arrange
                var dto = new ChangePasswordDTO
                {
                    Email = "user@example.com",
                    CurrentPassword = "oldpass",
                    NewPassword = "newpass123",
                    ConfirmNewPassword = "newpass123"
                };

                var user = new Employee
                {
                    Email = dto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword("oldpass")
                };

                _mockRepo.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

                // Act
                await _service.ChangePasswordAsync(dto);

                // Assert
                BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.Password).Should().BeTrue();
                _mockRepo.Verify(r => r.UpdateAsync(user), Times.Once);
            }

            [Fact]
            public async Task ChangePasswordAsync_ShouldThrow_WhenUserNotFound()
            {
                var dto = new ChangePasswordDTO
                {
                    Email = "notfound@example.com",
                    CurrentPassword = "irrelevant",
                    NewPassword = "1234",
                    ConfirmNewPassword = "1234"
                };

                _mockRepo.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((Employee)null);

                Func<Task> act = () => _service.ChangePasswordAsync(dto);

                await act.Should().ThrowAsync<Exception>()
                    .WithMessage("User not found.");
            }

            [Fact]
            public async Task ChangePasswordAsync_ShouldThrow_WhenCurrentPasswordIncorrect()
            {
                var dto = new ChangePasswordDTO
                {
                    Email = "user@example.com",
                    CurrentPassword = "wrongpass",
                    NewPassword = "newpass123",
                    ConfirmNewPassword = "newpass123"
                };

                var user = new Employee
                {
                    Email = dto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword("correctpass")
                };

                _mockRepo.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

                Func<Task> act = () => _service.ChangePasswordAsync(dto);

                await act.Should().ThrowAsync<Exception>()
                    .WithMessage("Current password is incorrect.");
            }

            [Fact]
            public async Task ChangePasswordAsync_ShouldThrow_WhenPasswordsDoNotMatch()
            {
                var dto = new ChangePasswordDTO
                {
                    Email = "user@example.com",
                    CurrentPassword = "oldpass",
                    NewPassword = "newpass1",
                    ConfirmNewPassword = "newpass2"
                };

                Func<Task> act = () => _service.ChangePasswordAsync(dto);

                await act.Should().ThrowAsync<Exception>()
                    .WithMessage("New password and confirmation do not match.");
            }
        }
    }

}
