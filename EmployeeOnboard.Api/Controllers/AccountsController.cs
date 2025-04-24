using AutoMapper;
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Infrastructure.Services;
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
        private readonly IAuthService _authService;
        private readonly ILogoutService _logoutService;

        public AccountsController(IRegisterService registerService, ILogger<AccountsController> logger, IMapper mapper, IAuthService authService, ILogoutService logoutService)
        {
            _registerService = registerService ?? throw new ArgumentNullException(nameof(registerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _authService = authService;
            _logoutService = logoutService;
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

       


        [HttpPost]
        [Route("auth/token")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var response = await _authService.LoginAsync(loginDTO);
                return Ok(response); // Return successful response
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { message = ex.Message }); // Handle authentication failure
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }


        

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            var (success, message) = await _logoutService.LogoutAsync(User);

            if (!success)
            {
                return BadRequest(message);
            }

            return Ok(message);
        }

    }
}
