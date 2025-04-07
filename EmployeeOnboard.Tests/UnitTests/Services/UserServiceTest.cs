
using AutoMapper;
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Domain.Enums;
using EmployeeOnboard.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeOnboard.Tests.UnitTests.Services
{
    public class UserServiceTest
    {
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock = new();
        private readonly Mock<ILogger<UserService>> _loggerMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly UserService _userService;

        public UserServiceTest()
        {
            _userService = new UserService(
                _employeeRepositoryMock.Object,
                _loggerMock.Object,
                _mapperMock.Object);
        }

        private void VerifyLog(LogLevel level, string containsMessage, Times times)
        {
            _loggerMock.Verify(
                x => x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(containsMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                times);
        }

        [Fact]
        public async Task DisableEmployeeAsync_Success_ReturnsTrue()
        {
            var employeeNumber = "EMP002";
            var employee = new Employee
            {
                EmployeeNumber = employeeNumber,
                Status = EmployeeStatus.Active
            };

            _employeeRepositoryMock
                .Setup(repo => repo.GetByEmployeeNumberAsync(employeeNumber))
                .ReturnsAsync(employee);

            _employeeRepositoryMock
                .Setup(repo => repo.UpdateAsync(employee))
                .Returns(Task.CompletedTask);

            var result = await _userService.DisableEmployeeAsync(employeeNumber);

            result.Should().BeTrue();
            employee.Status.Should().Be(EmployeeStatus.Inactive);
            VerifyLog(LogLevel.Information, "successfully disabled", Times.Once());
        }

        [Fact]
        public async Task DisableEmployeeAsync_EmployeeNotFound_ReturnsFalse()
        {
            var employeeNumber = "EMP999";
            _employeeRepositoryMock
                .Setup(repo => repo.GetByEmployeeNumberAsync(employeeNumber))
                .ReturnsAsync((Employee)null!);

            var result = await _userService.DisableEmployeeAsync(employeeNumber);

            result.Should().BeFalse();
            VerifyLog(LogLevel.Warning, "not found.", Times.Once());
        }

        [Fact]
        public async Task DisableEmployeeAsync_EmployeeAlreadyInactive_ReturnsFalse()
        {
            var employeeNumber = "EMP001";
            var employee = new Employee
            {
                EmployeeNumber = employeeNumber,
                Status = EmployeeStatus.Inactive
            };

            _employeeRepositoryMock
                .Setup(repo => repo.GetByEmployeeNumberAsync(employeeNumber))
                .ReturnsAsync(employee);

            var result = await _userService.DisableEmployeeAsync(employeeNumber);

            result.Should().BeFalse();
            VerifyLog(LogLevel.Warning, "already inactive", Times.Once());
        }

        [Fact]
        public async Task DisableEmployeeAsync_ExceptionThrown_ReturnsFalse()
        {
            var employeeNumber = "EMP003";
            var employee = new Employee
            {
                EmployeeNumber = employeeNumber,
                Status = EmployeeStatus.Active
            };

            _employeeRepositoryMock
                .Setup(repo => repo.GetByEmployeeNumberAsync(employeeNumber))
                .ReturnsAsync(employee);

            _employeeRepositoryMock
                .Setup(repo => repo.UpdateAsync(employee))
                .ThrowsAsync(new Exception("DB error"));

            var result = await _userService.DisableEmployeeAsync(employeeNumber);

            result.Should().BeFalse();
            VerifyLog(LogLevel.Error, "An error occurred", Times.Once());
        }

        [Fact]
        public async Task GetEmployeeByEmployeeNumberAsync_ShouldReturnEmployee_WhenExists()
        {
            // Arrange
            var employee = new Employee
            {
                FirstName = "Jane",
                MiddleName = "",
                LastName = "Doe",
                Email = "jane@example.com",
                PhoneNumber = "1234567890",
                Address = "Nairobi",
                Role = "Developer",
                Status = EmployeeStatus.Active,
                EmployeeNumber = "EMP001"
            };

            _employeeRepositoryMock.Setup(repo => repo.GetByEmployeeNumberAsync("EMP001"))
                .ReturnsAsync(employee);

            _mapperMock.Setup(mapper => mapper.Map<EmployeeResponseDTO>(It.IsAny<Employee>()))
                .Returns(new EmployeeResponseDTO
                {
                    FullName = "Jane Doe",
                    Email = "jane@example.com",
                    Role = "Developer",
                    Status = "Active",
                    EmployeeNumber = "EMP001",
                    PhoneNumber = "1234567890",
                    Address = "Nairobi"
                });

            // Act
            var result = await _userService.GetByEmployeeNumberAsync("EMP001");

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be("jane@example.com");
            result.Status.Should().Be("Active");
        }
        [Fact]
        public async Task EnableEmployeeAsync_EmployeeNotFound_ReturnsFalse()
        {
            _employeeRepositoryMock.Setup(repo => repo.GetByEmployeeNumberAsync("EMP001"))
                .ReturnsAsync((Employee?)null);

            var result = await _userService.EnableEmployeeAsync("EMP001");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task EnableEmployeeAsync_EmployeeAlreadyActive_ReturnsFalse()
        {
            var employee = new Employee { EmployeeNumber = "EMP001", Status = EmployeeStatus.Active };

            _employeeRepositoryMock.Setup(repo => repo.GetByEmployeeNumberAsync("EMP001"))
                .ReturnsAsync(employee);

            var result = await _userService.EnableEmployeeAsync("EMP001");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task EnableEmployeeAsync_Success_ReturnsTrue()
        {
            var employee = new Employee { EmployeeNumber = "EMP001", Status = EmployeeStatus.Inactive };

            _employeeRepositoryMock.Setup(repo => repo.GetByEmployeeNumberAsync("EMP001"))
                .ReturnsAsync(employee);

            _employeeRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Employee>()))
                .Returns(Task.CompletedTask);

            var result = await _userService.EnableEmployeeAsync("EMP001");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task EnableEmployeeAsync_ExceptionThrown_ReturnsFalse()
        {
            var employee = new Employee { EmployeeNumber = "EMP001", Status = EmployeeStatus.Inactive };

            _employeeRepositoryMock.Setup(repo => repo.GetByEmployeeNumberAsync("EMP001"))
                .ReturnsAsync(employee);

            _employeeRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Employee>()))
                .ThrowsAsync(new Exception("DB error"));

            var result = await _userService.EnableEmployeeAsync("EMP001");

            result.Should().BeFalse();
        }


    //    [Fact]
    //    public async Task GetAllActiveEmployeesAsync_ReturnsOnlyActiveEmployees()
    //    {
    //        var employees = new List<Employee>
    //        {
    //             new Employee { EmployeeNumber = "EMP001", FirstName = "Jane", MiddleName = "Serian", LastName = "Doe", Email = "jane@example.com", Role = "HR", Status = EmployeeStatus.Active },
    //             new Employee { EmployeeNumber = "EMP002", FirstName = "Mikael", LastName = "Smith", Email = "smith@example.com", Role = "Dev", Status = EmployeeStatus.Active }
    //        };

    //        _employeeRepositoryMock.Setup(repo => repo.GetAllActiveAsync())
    //            .ReturnsAsync(employees);

    //        _mapperMock.Setup(m => m.Map<List<Employee>>(It.IsAny<List<Employee>>()))
    //.Returns((List<Employee> source) => source.Select(e => new EmployeeResponseDTO
    //{
    //    FirstName = e.FirstName,
    //    LastName = e.LastName,
    //    Email = e.Email,
    //    EmployeeNumber = e.EmployeeNumber,
    //    Status = e.Status.ToString()
    //}).ToList());


    //        var result = await _userService.GetAllActiveEmployeesAsync();

    //        result.Should().HaveCount(2);
    //        result.All(e => e.Status == "Active").Should().BeTrue();
    //    }

    //    [Fact]
    //    public async Task GetAllActiveEmployeesAsync_NoActiveEmployees_ReturnsEmptyList()
    //    {
    //        _employeeRepositoryMock.Setup(repo => repo.GetAllActiveAsync())
    //            .ReturnsAsync(new List<Employee>());

    //        var result = await _userService.GetAllActiveEmployeesAsync();

    //        result.Should().BeEmpty();
    //    }
    }
}
