using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;



namespace EmployeeOnboard.Infrastructure.Services
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IUserRepository _userRepository;
        private readonly IResetPasswordTokenService _tokenService;
        private readonly IEmailService _emailService;

        public ForgotPasswordService(
            IUserRepository userRepository,
            IResetPasswordTokenService tokenService,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public async Task HandleAsync(ForgotPasswordRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                // Avoid revealing user existence
                return;
            }

            var token = await _tokenService.GenerateTokenAsync(user.Id.ToString());
            var resetLink = $"https://yourfrontend.com/reset-password?token={token}";

            await _emailService.SendResetPasswordEmail(user.Email, resetLink);
        }
    }       
}
