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
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly IEmailLogRepository _emailLogRepository;

    public RegisterService(IEmployeeRepository employeeRepository, ILogger<RegisterService> logger, INotificationService notificationService, IEmailLogRepository emailLogRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
        _notificationService = notificationService;
        _emailLogRepository = emailLogRepository;
        _mapper = mapper;
    }

    public async Task<(bool IsSuccess, string Message)> RegisterEmployeeAsync(Employee employee)
    {
        var validationResult = ValidateEmployee(employee);

        if (!validationResult.IsValid)
            return (false, validationResult.ToString());

        if (await EmployeeExistsAsync(employee.Email))
            return (false, "Email already exists");


        string employeeNumber = GenerateEmployeeNumber();

        string generatedPassword = GenerateRandomPassword(12);

        employee.EmployeeNumber = employeeNumber;
        employee.Password = BCrypt.Net.BCrypt.HashPassword(generatedPassword);

        await RegisterEmployeeToDatabaseAsync(employee);

        var emailSuccess = await SendWelcomeEmailAsync(employee, generatedPassword);

        if (!emailSuccess)
        {
            return (false, "Failed to send welcome email");
        }

        return (true, $"Employee registered successfully. Employee Number: {employeeNumber}, Password: {generatedPassword}");
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
    
    private ValidationResult ValidateEmployee(Employee employee)
    {
        var validator = new RegisterEmployeeValidator();
        ValidationResult validationResult = validator.Validate(employee);

        if (!validationResult.IsValid)
            _logger.LogWarning("Validation failed for employee registration. Errors: {Errors}", validationResult.ToString());

        return validationResult;
    }

    private async Task<bool> EmployeeExistsAsync(string email)
    {
        if (await _employeeRepository.ExistsByEmailAsync(email))
        {
            _logger.LogWarning("Attempt to register employee with existing email: {Email}", email);
            return true;
        }

        return false;
    }

    private string GenerateEmployeeNumber()
    {
        return Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
    }

    private async Task RegisterEmployeeToDatabaseAsync(Employee employee)
    {
        _logger.LogInformation("Registering employee. Employee Number: {EmployeeNumber}, Email: {Email}", employee.EmployeeNumber, employee.Email);
        await _employeeRepository.AddAsync(employee);
        _logger.LogInformation("Successfully registered employee. Employee ID: {EmployeeId}, Employee Number: {EmployeeNumber}, {Email}",
                               employee.Id, employee.EmployeeNumber, employee.Email);
    }

    private async Task<bool> SendWelcomeEmailAsync(Employee employee, string generatedPassword)
    {
        try
        {
            var emailDto = new EmailRequestDto
            {
                To = employee.Email,
                TemplateKey = "WelcomeEmail",
                Placeholders = new Dictionary<string, string>
            {
                { "FullName", $"{employee.FirstName} {employee.LastName}" },
                { "EmployeeNumber", employee.EmployeeNumber },
                { "Password", generatedPassword }
            }
            };

            await _notificationService.SendEmailAsync(emailDto);
            _logger.LogInformation("Welcome email sent to {Email}", employee.Email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to send email to {Email}. Error: {Message}", employee.Email, ex.Message);
            await LogEmailFailureAsync(employee, generatedPassword, ex);
            return false;
        }
    }

    private async Task LogEmailFailureAsync(Employee employee, string generatedPassword, Exception ex)
    {
        var log = new EmailLog
        {
            RecipientEmail = employee.Email,
            Subject = "Welcome to the Team!",
            Body = $"Employee Number: {employee.EmployeeNumber}, Password: {generatedPassword}",
            IsSuccess = false,
            ErrorMessage = ex.Message
        };

        await _emailLogRepository.LogEmailAsync(log);
    }

}
