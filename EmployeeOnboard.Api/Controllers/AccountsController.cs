using AutoMapper;
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeOnboard.Api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IRegisterService _registerService;
        private readonly ILogger<AccountsController> _logger;
        private readonly IMapper _mapper;
        private readonly IResetPasswordService _resetPasswordService;

        public AccountsController(IRegisterService registerService, ILogger<AccountsController> logger, IMapper mapper, IResetPasswordService resetPasswordService)
        {
            _registerService = registerService ?? throw new ArgumentNullException(nameof(registerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _resetPasswordService = resetPasswordService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployeeDTO request)
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
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO request)
        {
            if (request == null)
            {
                _logger.LogWarning("Reset password request is null.");
                return BadRequest(new { success = false, message = "Invalid request." });
            }
            // Validate that all required fields are provided
            if (string.IsNullOrEmpty(request.Email) ||
                string.IsNullOrEmpty(request.CurrentPassword) ||
                string.IsNullOrEmpty(request.NewPassword) ||
                string.IsNullOrEmpty(request.ConfirmPassword))

            {
                _logger.LogWarning("Missing required fields for password reset.");
                return BadRequest(new { success = false, message = "All fields are required." });
            }

            // Validate password and confirm password match
            if (request.NewPassword != request.ConfirmPassword)
            {
                _logger.LogWarning("Password and Confirm Password do not match for {Email}.", request.Email);
                return BadRequest(new { success = false, message = "Passwords do not match." });
            }

            // Call the service to reset the password using the entire DTO
            var result = await _resetPasswordService.ResetPasswordAsync(request);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Password reset failed for {Email}: {Message}", request.Email, result.Message);
                return BadRequest(new { success = false, message = result.Message });
            }

            return Ok(new { success = true, message = result.Message });
        }
    }
}
