﻿using AutoMapper;
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeOnboard.Tests.UnitTests.Services;

public class RegisterServiceTest
{
    private readonly RegisterService _registerService;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock = new();
    private readonly Mock<ILogger<RegisterService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    public RegisterServiceTest()
    {
        _registerService = new RegisterService(_employeeRepositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
    }
    [Fact]
    public async Task RegisterEmployeeAsync_ShouldReturnError_WhenEmailExists()
    {
        //Arrange
        var dto = new Employee { Email = "existing@example.com", FirstName = "John", MiddleName = "Jane", LastName = "Doe", PhoneNumber = "254712345678", Role = "Developer" };
        _employeeRepositoryMock.Setup(repo => repo.ExistsByEmailAsync(dto.Email)).ReturnsAsync(true);

        //Act
        var result = await _registerService.RegisterEmployeeAsync(dto);

        //Assert
        result.Should().Be((false, "Email already exists"));
    }
    [Fact]
    public async Task RegisterEmployeeAsync_ShouldRegisterEmployee_WhenValidInput()
    {
        var dto = new Employee { Email = "new@example.com", FirstName = "Suzanne", MiddleName = "Jane", LastName = "Suzane", PhoneNumber = "254712345678", Role = "Developer" };
        _employeeRepositoryMock.Setup(repo => repo.ExistsByEmailAsync(dto.Email)).ReturnsAsync(false);
        _employeeRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Employee>())).Returns(Task.CompletedTask);


        var result = await _registerService.RegisterEmployeeAsync(dto);

        result.Message.Should().StartWith("Employee registered successfully. Employee Number: ");
        _employeeRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Employee>()), Times.Once);

    }
}
