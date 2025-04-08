using Castle.Core.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Infrastructure.Services;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using Microsoft.Extensions.Logging;



namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
    public class ResetPasswordServiceTests
    {
        private readonly Mock<IResetPasswordRepository> _resetPasswordRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;  
        private readonly Mock<ILogger<ResetPasswordService>> _loggerMock;
        private readonly ResetPasswordService _resetPasswordService;

        public ResetPasswordServiceTests()
        {
            _resetPasswordRepositoryMock = new Mock<IResetPasswordRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<ResetPasswordService>>();

            _resetPasswordService = new ResetPasswordService(
                _resetPasswordRepositoryMock.Object,
                _userRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact] 
        public async Task ResetPasswordAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var request = new ResetPasswordDTO { Email = "test@example.com" };
            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(request.Email)).ReturnsAsync((User)null);

            // Act
            var result = await _resetPasswordService.ResetPasswordAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found.", result.Message);
        }

        [Fact]
        public async Task ResetPasswordAsync_PasswordsDoNotMatch_ReturnsFailure()
        {
            // Arrange
            var request = new ResetPasswordDTO
            {
                Email = "test@example.com",
                CurrentPassword = "oldPass",
                NewPassword = "newPass1",
                ConfirmPassword = "newPass2"
            };
            var user = new User { Email = request.Email, PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldPass") };

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(request.Email)).ReturnsAsync(user);

            // Act
            var result = await _resetPasswordService.ResetPasswordAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Passwords do not match.", result.Message);
        }

        [Fact]
        public async Task ResetPasswordAsync_InvalidCurrentPassword_ReturnsFailure()
        {
            // Arrange
            var request = new ResetPasswordDTO
            {
                Email = "test@example.com",
                CurrentPassword = "wrongPass",
                NewPassword = "newPass",
                ConfirmPassword = "newPass"
            };
            var user = new User { Email = request.Email, PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctPass") };

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(request.Email)).ReturnsAsync(user);

            // Act
            var result = await _resetPasswordService.ResetPasswordAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Current password is incorrect.", result.Message);
        }

        [Fact]
        public async Task ResetPasswordAsync_UpdatePasswordFails_ReturnsFailure()
        {
            // Arrange
            var request = new ResetPasswordDTO
            {
                Email = "test@example.com",
                CurrentPassword = "password",
                NewPassword = "newPassword",
                ConfirmPassword = "newPassword"
            };
            var user = new User { Email = request.Email, PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(request.Email)).ReturnsAsync(user);
            _resetPasswordRepositoryMock.Setup(r =>
                r.UpdatePasswordAsync(request.Email, It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var result = await _resetPasswordService.ResetPasswordAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Password reset failed. Please try again.", result.Message);
        }

        [Fact]
        public async Task ResetPasswordAsync_SuccessfulReset_ReturnsSuccess()
        {
            // Arrange
            var request = new ResetPasswordDTO
            {
                Email = "test@example.com",
                CurrentPassword = "password",
                NewPassword = "newPassword",
                ConfirmPassword = "newPassword"
            };
            var user = new User { Email = request.Email, PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(request.Email)).ReturnsAsync(user);
            _resetPasswordRepositoryMock.Setup(r =>
                r.UpdatePasswordAsync(request.Email, It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _resetPasswordService.ResetPasswordAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Password reset successfully.", result.Message);
        }
    }
}

