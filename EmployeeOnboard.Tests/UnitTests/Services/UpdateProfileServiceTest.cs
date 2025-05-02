using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.UOW;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Infrastructure.Services.Employees;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;


namespace EmployeeOnboard.Tests.UnitTests.Services
{
    public class UpdateProfileServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<UpdateProfileService>> _loggerMock;
        private readonly UpdateProfileService _service;

        public UpdateProfileServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<UpdateProfileService>>();
            _service = new UpdateProfileService(_loggerMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task UpdateProfileAsync_Should_ReturnFailure_When_UserNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Employee.GetByEmailAsync(It.IsAny<string>()))
                           .ReturnsAsync((Employee)null);

            var dto = new UpdateProfileDTO();

            // Act
            var result = await _service.UpdateProfileAsync(dto, "nonexistent@example.com", "Employee");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("User not found");
        }

        [Fact]
        public async Task UpdateProfileAsync_Should_UpdateEmployeeFields_When_RoleIsEmployee()
        {
            // Arrange
            var existingUser = new Employee
            {
                Id = Guid.NewGuid(),
                Email = "employee@example.com"
            };

            _unitOfWorkMock.Setup(uow => uow.Employee.GetByEmailAsync("employee@example.com"))
                           .ReturnsAsync(existingUser);

            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
                           .ReturnsAsync(1);

            var dto = new UpdateProfileDTO
            {
                PhoneNumber = "123456789",
                AltPhoneNumber = "987654321",
                Address = "123 Main St",
                ProfileImgUrl = "http://image.com/pic.jpg"
            };

            // Act
            var result = await _service.UpdateProfileAsync(dto, "employee@example.com", "Employee");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.PhoneNumber.Should().Be(dto.PhoneNumber);
            result.Data.AltPhoneNumber.Should().Be(dto.AltPhoneNumber);
            result.Data.Address.Should().Be(dto.Address);
            result.Data.ProfileImgUrl.Should().Be(dto.ProfileImgUrl);
        }

        [Fact]
        public async Task UpdateProfileAsync_Should_UpdateAdminFields_When_RoleIsAdmin()
        {
            // Arrange
            var existingUser = new Employee
            {
                Id = Guid.NewGuid(),
                Email = "admin@example.com"
            };

            _unitOfWorkMock.Setup(uow => uow.Employee.GetByEmailAsync("admin@example.com"))
                           .ReturnsAsync(existingUser);

            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
                           .ReturnsAsync(1);

            var dto = new UpdateProfileDTO
            {
                FirstName = "Admin",
                MiddleName = "Middle",
                LastName = "User",
                Email = "admin_new@example.com",
                Role = "Admin",
                PhoneNumber = "111111111",
                AltPhoneNumber = "222222222",
                Address = "456 Admin Rd",
                ProfileImgUrl = "http://image.com/admin.jpg"
            };

            // Act
            var result = await _service.UpdateProfileAsync(dto, "admin@example.com", "Admin");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.FirstName.Should().Be(dto.FirstName);
            result.Data.Email.Should().Be(dto.Email);
            result.Data.Role.Should().Be(dto.Role);
        }

        [Fact]
        public async Task UpdateProfileAsync_Should_ReturnFailure_When_NoChangesSaved()
        {
            // Arrange
            var existingUser = new Employee
            {
                Id = Guid.NewGuid(),
                Email = "employee@example.com"
            };

            _unitOfWorkMock.Setup(uow => uow.Employee.GetByEmailAsync("employee@example.com"))
                           .ReturnsAsync(existingUser);

            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
                           .ReturnsAsync(0); // Simulate no rows affected

            var dto = new UpdateProfileDTO
            {
                PhoneNumber = "123456789",
                AltPhoneNumber = "987654321",
                Address = "123 Main St",
                ProfileImgUrl = "http://image.com/pic.jpg"
            };

            // Act
            var result = await _service.UpdateProfileAsync(dto, "employee@example.com", "Employee");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("No changes were saved.");
        }
    }
}
    

