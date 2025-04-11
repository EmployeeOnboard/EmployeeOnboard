using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeOnboard.Api.Controllers.AuthController
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogoutService _logoutService;

        public AuthController(IAuthService authService, ILogoutService logoutService)
        { 
            _authService = authService;
            _logoutService = logoutService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var response = await _authService.LoginAsync(loginDTO);
            if (!response.Success)
            {
                return Unauthorized(response);
            }
            return Ok(response);
        }


        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Invalid user");

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest("Invalid user ID format");

            var result = await _logoutService.LogoutAsync(userId);

            if (!result) return BadRequest("Logout failed");

            return Ok("Logged out successfully");
        }

    }
}
