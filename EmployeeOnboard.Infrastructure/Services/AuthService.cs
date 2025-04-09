using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Infrastructure.Services
{
    public class AuthService
    {
        private readonly IEmailService _emailService;
        private object resetLink;

        public AuthService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<(bool IsSuccess, string Message)> SendPasswordResetTokenAsync(string email)
        {
            // your logic for finding user + generating resetLink

            await _emailService.SendAsync(email, "Reset Your Password", $"Click here: {resetLink}");

            return (true, "Reset link sent.");
        }
    }
}
