//using EmployeeOnboard.Application.DTOs;
//using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
//using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
//using EmployeeOnboard.Domain.Entities;
//using EmployeeOnboard.Infrastructure.Services.Notification;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Net.Mail;
//using System.Collections.Generic;    
//using System.Threading.Tasks;
//using EmployeeOnboard.Domain.Models;
//using EmployeeOnboard.Infrastructure.Repositories;
//using EmployeeOnboard.Application.DTOs.PasswordManagementDTO;
//using EmployeeOnboard.Domain.Enums;


//namespace EmployeeOnboard.Infrastructure.Services.PasswordManagementService
//{
//    public class ForgotPasswordService : IForgotPasswordService
//    {
//        private readonly IEmployeeRepository _employeeRepository;
//        private readonly INotificationService _notificationService;
//        private readonly IConfiguration _configuration;
//        private readonly IEmailTemplateService _emailTemplateService;
//        private readonly IForgotPasswordTokenRepository _forgotPasswordTokenRepository;

//        public ForgotPasswordService(
//            IEmployeeRepository employeeRepository,
//            INotificationService notificationService,
//            IConfiguration configuration,
//            IEmailTemplateService emailTemplateService,
//            IForgotPasswordTokenRepository forgotPasswordTokenRepository)
//        {
//            _employeeRepository = employeeRepository;
//            _notificationService = notificationService;
//            _configuration = configuration;
//            _emailTemplateService = emailTemplateService;
//            _forgotPasswordTokenRepository = forgotPasswordTokenRepository;
//        }


//        public async Task ForgotPasswordAsync(string email)
//        {
//            // Step 1: Get the user (employee) from the database
//            var user = await _employeeRepository.GetByEmailAsync(email);
//            if (user == null)
//                throw new Exception("User with that email does not exist.");

//            // Step 2: Generate the token and expiry time
//            var token = Guid.NewGuid().ToString();
//            var expiry = DateTime.UtcNow.AddHours(1); // 1-hour expiry

//            var existingToken = await _forgotPasswordTokenRepository.GetByEmployeeIdAsync(user.Id);

//            if (existingToken == null)
//            {
//                var newToken = new ForgotPasswordToken
//                {
//                    PasswordResetToken = token,
//                    PasswordResetTokenExpiry = expiry,
//                    EmployeeId = user.Id
//                };

//                await _forgotPasswordTokenRepository.AddAsync(newToken);
//            }
//            else
//            {
//                existingToken.PasswordResetToken = token;
//                existingToken.PasswordResetTokenExpiry = expiry;


//                await _forgotPasswordTokenRepository.UpdateAsync(existingToken);
//            }


//            // Step 3: Build the reset link
//            var resetLink = $"{_configuration["AppSettings:FrontendBaseUrl"]}/reset-password?token={token}&email={email}";

//            // Step 4: Load and fill the email template
//            var template = _emailTemplateService.GetTemplate(EmailTemplateType.ForgotPassword);
//            var placeholders = new Dictionary<string, string>
//            {
//                { "UserName", $"{user.FirstName} {user.LastName}" },
//                { "ResetLink", resetLink },
//                { "ExpiryTime", "1 hour" }
//            };

//            var (subject, body) = _emailTemplateService.ReplacePlaceholders(template.Subject, template.Body, placeholders);

//            // Step 5: Send the email
//            var emailRequest = new EmailRequestDto
//            {
//                To = user.Email,
//                TemplateKey = "ForgotPassword", // This is the key in your appsettings.json
//                Placeholders = placeholders
//            };

//            // Step 6: Send email via notification service
//            await _notificationService.SendEmailAsync(emailRequest);
//        }

//        public async Task ResetPasswordAsync(ResetPasswordRequestDTO request)
//        {
//            if (request.NewPassword != request.ConfirmPassword)
//                throw new Exception("Passwords do not match.");

//            var user = await _employeeRepository.GetByEmailAsync(request.Email);
//            if (user == null)
//                throw new Exception("User not found.");

//            // Hash the new password using Bcrypt
//            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

//            await _employeeRepository.UpdateAsync(user);

//        }

//    }

//} 
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.DTOs.PasswordManagementDTO;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Infrastructure.Services.Notification;
using Microsoft.Extensions.Configuration;

namespace EmployeeOnboard.Infrastructure.Services.PasswordManagementService
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;
        private readonly EmailTemplateService _emailTemplateService;
        private readonly IForgotPasswordTokenRepository _forgotPasswordTokenRepository;

        public ForgotPasswordService(
            IEmployeeRepository employeeRepository,
            INotificationService notificationService,
            IConfiguration configuration,
            EmailTemplateService emailTemplateService,
            IForgotPasswordTokenRepository forgotPasswordTokenRepository)
        {
            _employeeRepository = employeeRepository;
            _notificationService = notificationService;
            _configuration = configuration;
            _emailTemplateService = emailTemplateService;
            _forgotPasswordTokenRepository = forgotPasswordTokenRepository;
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _employeeRepository.GetByEmailAsync(email)
                       ?? throw new Exception("User with that email does not exist.");

            var token = await GenerateAndStoreResetTokenAsync(user.Id);
            var resetLink = BuildResetLink(token, email);
            var placeholders = BuildEmailPlaceholders(user.FirstName, user.LastName, resetLink);

            var emailRequest = new EmailRequestDto
            {
                To = user.Email,
                TemplateKey = "ForgotPassword",
                Placeholders = placeholders
            };

            await _notificationService.SendEmailAsync(emailRequest);
        }

        public async Task ResetPasswordAsync(ResetPasswordRequestDTO request)
        {
            if (request.NewPassword != request.ConfirmPassword)
                throw new Exception("Passwords do not match.");

            var user = await _employeeRepository.GetByEmailAsync(request.Email)
                       ?? throw new Exception("User not found.");

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _employeeRepository.UpdateAsync(user);
        }

        private async Task<string> GenerateAndStoreResetTokenAsync(Guid employeeId)
        {
            var token = Guid.NewGuid().ToString();
            var expiry = DateTime.UtcNow.AddHours(1);

            var existingToken = await _forgotPasswordTokenRepository.GetByEmployeeIdAsync(employeeId);

            if (existingToken == null)
            {
                var newToken = new ForgotPasswordToken
                {
                    PasswordResetToken = token,
                    PasswordResetTokenExpiry = expiry,
                    EmployeeId = employeeId
                };
                await _forgotPasswordTokenRepository.AddAsync(newToken);
            }
            else
            {
                existingToken.PasswordResetToken = token;
                existingToken.PasswordResetTokenExpiry = expiry;
                await _forgotPasswordTokenRepository.UpdateAsync(existingToken);
            }

            return token;
        }

        private string BuildResetLink(string token, string email)
        {
            var baseUrl = _configuration["AppSettings:FrontendBaseUrl"];
            return $"{baseUrl}/reset-password?token={token}&email={email}";
        }

        private Dictionary<string, string> BuildEmailPlaceholders(string firstName, string lastName, string resetLink)
        {
            return new Dictionary<string, string>
            {
                { "UserName", $"{firstName} {lastName}" },
                { "ResetLink", resetLink },
                { "ExpiryTime", "1 hour" }
            };
        }
    }
}
