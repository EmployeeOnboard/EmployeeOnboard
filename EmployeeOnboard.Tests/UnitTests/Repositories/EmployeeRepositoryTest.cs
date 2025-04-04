using AutoMapper;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Infrastructure.Data;
using EmployeeOnboard.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EmployeeOnboard.Tests.UnitTests.Repositories;

public class EmployeeRepositoryTest
{
    private readonly EmployeeRepository _employeeRepository;
    private readonly Mock<ApplicationDbContext> _dbContextMock;

    public EmployeeRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        var context = new ApplicationDbContext(options);
        _employeeRepository = new EmployeeRepository(context);
    }

    [Fact]
    public async Task AddAsync_Should_AddEmployee_ToDatabase() // ensure that the employee is stored properly
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            MiddleName = "",
            LastName = "Doe",
            Email = "john.doe@example.com",
            EmployeeNumber = "EMP001",
            Password = "hashedpassword",
            Role = "Developer",
            CreatedAt = DateTime.UtcNow
        };

        await _employeeRepository.AddAsync(employee);
        var result = await _employeeRepository.GetByIdAsync(employee.Id);

        result.Should().NotBeNull();
        result.Email.Should().Be(employee.Email);
    }

    [Fact]
    public async Task ExistsByEmailAsync_Should_ReturnTrue_IfEmailExists() // should return true for existing emails
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            MiddleName = "",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            EmployeeNumber = "EMP002",
            Password = "hashedpassword",
            Role = "HR",
            CreatedAt = DateTime.UtcNow
        };

        await _employeeRepository.AddAsync(employee);

        var exists = await _employeeRepository.ExistsByEmailAsync("jane.doe@example.com");

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_Should_ReturnFalse_IfEmailDoesNotExist() //verifies that it returns false for a nonexistent email
    {
        var exists = await _employeeRepository.ExistsByEmailAsync("nonexistent@example.com");

        exists.Should().BeFalse();
    }
}
