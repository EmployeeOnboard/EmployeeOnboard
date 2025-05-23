﻿using AutoMapper;
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Application.DTOs.PasswordManagementDTO;
using EmployeeOnboard.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;


namespace EmployeeOnboard.Api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IRegisterService _registerService;
        private readonly ILogger<AccountsController> _logger;
        private readonly IMapper _mapper;
        private readonly IChangePassword _changePasswordService;
        private readonly IForgotPasswordService _forgotPasswordService;
        private readonly IAuthService _authService;
        private readonly ILogoutService _logoutService;
        private readonly IUpdateProfileService _updateProfileService;



        public AccountsController(IRegisterService registerService, ILogger<AccountsController> logger, IMapper mapper, IChangePassword changePasswordService, IForgotPasswordService forgotPasswordService, IAuthService authService, ILogoutService logoutService, IUpdateProfileService updateProfileService)
        {
            _registerService = registerService ?? throw new ArgumentNullException(nameof(registerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _changePasswordService = changePasswordService;
            _forgotPasswordService = forgotPasswordService;
            _authService = authService;
            _logoutService = logoutService;
            _updateProfileService = updateProfileService;

        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployeeDTO request)
        {
            try
            {
                var employee = _mapper.Map<Employee>(request);

                var result = await _registerService.RegisterEmployeeAsync(employee);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Employee registration failed: {Message}", result.Message);
                    return BadRequest(new { success = false, message = result.Message });
                }

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred during employee registration.");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            await _changePasswordService.ChangePasswordAsync(dto);
            return Ok(new { message = "Password changed successfully." });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            await _forgotPasswordService.ForgotPasswordAsync(dto.Email);
            return Ok("Reset link sent if email exists.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO request)
        {
            await _forgotPasswordService.ResetPasswordAsync(request);
            return Ok(new { message = "Password has been reset successfully." });
        }
        


        [HttpPost]
        [Route("auth/token")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? "No inner exception";
                _logger.LogError(ex, "Unhandled exception in login controller");

                return StatusCode(500, new
                {
                    Message = "Internal Server Error",
                    Error = ex.Message,
                    InnerError = innerMessage
                });
            }

        }

        [HttpPost]
        [Route("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var result = await _logoutService.LogoutAsync(User);  // User is the ClaimsPrincipal
            if (!result.Success)
            {
                _logger.LogWarning("Logout failed for user {UserId}: {Message}", User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, result.Message);
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }

        [Authorize(Roles = "Employee, Admin, SuperAdmin")]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDTO dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
            {
                _logger.LogWarning("user email or role not found in claims");

                return Unauthorized("Invalid token: missing email or role");
            }

            var result = await _updateProfileService.UpdateProfileAsync(dto, email, role);

            if (result == null)
            {
                _logger.LogError("Failed to update profile for user: {Message}", result.Message);
                return BadRequest(new { success = false, message = result.Message });
            }

            return Ok(new { success = true, message = result.Message, data = result.Data });
        }
    }
}
