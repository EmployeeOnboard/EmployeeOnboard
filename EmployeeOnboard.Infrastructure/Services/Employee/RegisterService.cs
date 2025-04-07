using AutoMapper;
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Application.Validators;
using EmployeeOnboard.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using System.Text;

namespace EmployeeOnboard.Infrastructure.Services;

public class RegisterService : IRegisterService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<RegisterService> _logger;
    private readonly IMapper _mapper;

    public RegisterService(IEmployeeRepository employeeRepository, ILogger<RegisterService> logger, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
        _mapper = mapper;
    }

    private string GenerateRandomPassword(int length = 12)
    {
        const string upperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string specialChars = "!@#$%^&*_-+=<>?";


        string prefix = "EMP";

        int remainingLength = length - prefix.Length;

        if (remainingLength < 0)
            throw new ArgumentException("Password length must be greater than the length of the prefix.");

        Random random = new Random();

        var passwordBuilder = new StringBuilder(prefix);

        for (int i = 0; i < 4; i++)
        {
            passwordBuilder.Append(upperCaseChars[random.Next(upperCaseChars.Length)]);
        }

        for (int i = 0; i < 4; i++)
        {
            passwordBuilder.Append(digits[random.Next(digits.Length)]);
        }

        for (int i = 0; i < 2; i++)
        {
            passwordBuilder.Append(specialChars[random.Next(specialChars.Length)]);
        }

        int additionalCharsCount = remainingLength - (4 + 4 + 2);

        const string allChars = upperCaseChars + lowerCaseChars + digits + specialChars;


        for (int i = 0; i < additionalCharsCount; i++)
        {
            passwordBuilder.Append(allChars[random.Next(allChars.Length)]);
        }

        string passwordWithoutPrefix = passwordBuilder.ToString().Substring(prefix.Length); // Remove the prefix
        var shuffledPassword = new string(passwordWithoutPrefix.OrderBy(x => random.Next()).ToArray());

        return prefix + shuffledPassword;
    }

    public async Task<(bool IsSuccess, string Message)> RegisterEmployeeAsync(Employee employee)
    {
        var validator = new RegisterEmployeeValidator();

        ValidationResult validationResult = validator.Validate(employee);

        if (!validationResult.IsValid) //validating the input for employee before registering them
        {
            _logger.LogWarning("Validation failed for employee registration. Errors: {Errors}", validationResult.ToString());
            return (false, validationResult.ToString());
        }

        if (await _employeeRepository.ExistsByEmailAsync(employee.Email))
        {
            _logger.LogWarning("Attempt to register employee with existing email: {Email}", employee.Email);
            return (false, "Email already exists");
        }

        string employeeNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

        string generatedPassword = GenerateRandomPassword(12);

        //var employee = _mapper.Map<Employee>(request);
        employee.EmployeeNumber = employeeNumber;
        employee.Password = BCrypt.Net.BCrypt.HashPassword(generatedPassword);


        _logger.LogInformation("Registering employee. Employee Number: {EmployeeNumber}, Email: {Email}", employee.EmployeeNumber, employee.Email);

        await _employeeRepository.AddAsync(employee);

        _logger.LogInformation("Successfully registered employee. Employee ID: {EmployeeId}, Employee Number: {EmployeeNumber}, {Email}",
                           employee.Id, employee.EmployeeNumber, employee.Email);

        return (true, $"Employee registered successfully. Employee Number: {employeeNumber}, Password: {generatedPassword}");
    }
}
