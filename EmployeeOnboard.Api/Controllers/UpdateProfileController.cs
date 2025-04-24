using System.Security.Claims;
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeOnboard.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UpdateProfileController : ControllerBase
    {
        private readonly IUpdateProfileService _updateProfileService;
        private readonly ILogger<UpdateProfileController> _logger;

        public UpdateProfileController(IUpdateProfileService updateProfileService, ILogger<UpdateProfileController> logger)
        {
            _updateProfileService = updateProfileService;
            _logger = logger;
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

