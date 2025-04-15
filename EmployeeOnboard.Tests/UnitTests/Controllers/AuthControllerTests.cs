using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces;
using Moq;
using Xunit;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EmployeeOnboard.Api.Controllers;
using AutoMapper;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Logging;




namespace EmployeeOnboard.Tests.Services
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ILogoutService> _mockLogoutService;
        private readonly AccountsController _controller;
        private readonly IMapper _mapper;

        public AuthControllerTests()
        {
            // Mocking the services
            _mockAuthService = new Mock<IAuthService>();
            _mockLogoutService = new Mock<ILogoutService>();
            _mapper = new Mock<IMapper>().Object;  // Fix here: using .Object to get the IMapper instance

            var mockRegisterService = new Mock<IRegisterService>();
            var mockLogger = new Mock<ILogger<AccountsController>>();

            // Creating the controller instance
            _controller = new AccountsController(
                mockRegisterService.Object,
                mockLogger.Object,
                _mapper,  // Now passing the correct IMapper object
                _mockAuthService.Object,
                _mockLogoutService.Object
            );
        }



        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var loginDTO = new LoginDTO
            {
                Email = "test@example.com",
                Password = "correct_password"
            };

            var authResponse = new AuthResponseDTO
            {
                Success = true,
                Token = "some_token",
                RefreshToken = "some_refresh_token",
                Message = "Successfully logged in"
            };

            _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<LoginDTO>())).ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Login(loginDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponseDTO>(okResult.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDTO = new LoginDTO
            {
                Email = "test@example.com",
                Password = "wrong_password"
            };

            var authResponse = new AuthResponseDTO
            {
                Success = false,
                Message = "Invalid credentials"
            };

            _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<LoginDTO>())).ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Login(loginDTO);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<AuthResponseDTO>(unauthorizedResult.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task Logout_ValidUser_ReturnsOkResult()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Mock that the user logs out successfully
            _mockLogoutService.Setup(x => x.LogoutAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            // Simulate user ID claim (you can mock the user claims for the test)
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await _controller.Logout();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Logged out successfully", okResult.Value);
        }

        [Fact]
        public async Task Logout_InvalidGuid_ReturnsBadRequest()
        {
            // Arrange
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "invalid_guid") };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await _controller.Logout();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid user ID format", badRequestResult.Value);
        }

        [Fact]
        public async Task Logout_LogoutFails_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockLogoutService.Setup(x => x.LogoutAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await _controller.Logout();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Logout failed", badRequestResult.Value);
        }

    }
}
