using AutoMapper;
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Application.DTOs.PasswordManagementDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EmployeeOnboard.Infrastructure.Services.PasswordManagementService;


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

        public AccountsController(IRegisterService registerService, ILogger<AccountsController> logger, IMapper mapper, IChangePassword changePasswordService, IForgotPasswordService forgotPasswordService)
        {
            _registerService = registerService ?? throw new ArgumentNullException(nameof(registerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _changePasswordService = changePasswordService;
            _forgotPasswordService = forgotPasswordService;
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

    }
}
