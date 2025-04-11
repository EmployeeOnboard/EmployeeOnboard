using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Reflection;
using System;

namespace EmployeeOnboard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly IAuthService _authService; 

        public ForgotPasswordController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO request)
        {
            var result = await _authService.SendPasswordResetTokenAsync(request.Email);

            if (!result.IsSuccess)
                return BadRequest(new { success = false, message = result.Message });

            return Ok(new { success = true, message = "Password reset link sent to your email." });
        }
    }
}
