using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EmployeeOnboard.Api.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class ResetPasswordController : ControllerBase
    {
        private readonly IResetPasswordService _resetPasswordService;
        private readonly ILogger<ResetPasswordController> _logger;

        public ResetPasswordController(IResetPasswordService resetPasswordService, ILogger<ResetPasswordController> logger)
        {
            _resetPasswordService = resetPasswordService ?? throw new ArgumentNullException(nameof(resetPasswordService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
