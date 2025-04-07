using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeOnboard.Api.Controllers
{
    [Route("api/employee")]
    [ApiController]
    public class ManageUsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public ManageUsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("disable/{employeeNumber}")]
        public async Task<IActionResult> DisableEmployee([FromRoute] string employeeNumber)
        {
            var result = await _userService.DisableEmployeeAsync(employeeNumber);

            if (!result)
                return NotFound($"Employee with employee number {employeeNumber} not found or already disabled");

            return Ok($"Employee with employee number {employeeNumber} is disabled.");
        }

        [HttpGet("get-employee/{employeeNumber}")]
        public async Task<IActionResult> GetByEmployeeNumber([FromRoute] string employeeNumber)
        {
            var result = await _userService.GetByEmployeeNumberAsync(employeeNumber);
            if (result == null)
            {
                return NotFound($"No employee found with employee number: {employeeNumber}");
            }
            return Ok(result);
        }

        [HttpPut("enable/{employeeNumber}")]
        public async Task<IActionResult> EnableEmployee([FromRoute] string employeeNumber)
        {
            var result = await _userService.EnableEmployeeAsync(employeeNumber);
            if (!result)
                return BadRequest($"Unable to enable employee with employee number {employeeNumber}");

            return Ok($"Employee with employee number {employeeNumber} is enabled.");
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _userService.GetAllActiveEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("email")]
        public async Task<IActionResult> GetEmployeeByEmail([FromQuery] string email)
        {
            var employee = await _userService.GetEmployeeByEmailAsync(email);

            if (employee == null)
                return NotFound($"Employee with email {email} not found.");

            return Ok(employee);
        }      
    }
}
