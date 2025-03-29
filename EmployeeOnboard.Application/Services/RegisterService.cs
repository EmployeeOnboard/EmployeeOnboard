using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Application.Validators;
using EmployeeOnboard.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace EmployeeOnboard.Application.Services;

public class RegisterService : IRegisterService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<RegisterService> _logger;

    public RegisterService(IEmployeeRepository employeeRepository, ILogger<RegisterService> logger)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    private string GenerateRandomPassword()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 8);
    }

    public async Task<(bool IsSuccess, string Message)> RegisterEmployeeAsync(RegisterEmployeeDTO dto)
    {
        var validator = new RegisterEmployeeValidator();

        ValidationResult validationResult = validator.Validate(dto);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed: {Errors}", validationResult.ToString());
            return (false, validationResult.ToString());
        }

        if (await _employeeRepository.ExistsByEmailAsync(dto.Email))
        {
            _logger.LogWarning("Attempt to register employee with existing email: {Email}", dto.Email);
            return (false, "Email already exists");
        }

        string employeeNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

        string generatedPassword = GenerateRandomPassword();

        string fullName = $"{dto.FirstName} {dto.MiddleName}{dto.LastName}".Trim();

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Email = dto.Email,
            EmployeeNumber = employeeNumber,
            PhoneNumber = dto.PhoneNumber,
            Password = BCrypt.Net.BCrypt.HashPassword(generatedPassword),
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow
        };

        await _employeeRepository.AddAsync(employee);
        _logger.LogInformation("Successfully registered employee: {Email}", dto.Email);

        return (true, $"Employee registered successfully. Employee Number: {employeeNumber}");
    }
}
