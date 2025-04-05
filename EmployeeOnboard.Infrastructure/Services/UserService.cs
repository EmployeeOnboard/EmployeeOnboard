using AutoMapper;
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace EmployeeOnboard.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;
    public UserService(IEmployeeRepository employeeRepository, ILogger<UserService> logger, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<bool> DisableEmployeeAsync(string employeeNumber)
    {
        var employee = await _employeeRepository.GetByEmployeeNumberAsync(employeeNumber);

        if (employee == null)
        {
            _logger.LogWarning("Employee with EmployeeNumber {EmployeeNumber} not found.", employeeNumber);
            return false;
        }

        if (employee.EmployeeNumber == "SUPERADMIN01")
        {
            _logger.LogWarning("Attempt to disable SuperAdmin with EmployeeNumber {EmployeeNumber} was blocked.", employeeNumber);
            return false;
        }


        if (employee.Status == EmployeeStatus.Inactive)
        {
            _logger.LogWarning("Employee with EmployeeNumber {EmployeeNumber} is already inactive.", employeeNumber);
            return false;
        }

        employee.Status = EmployeeStatus.Inactive;

        try
        {
            await _employeeRepository.UpdateAsync(employee);
            _logger.LogInformation("Employee with EmployeeNumber {EmployeeNumber} has been successfully disabled.", employeeNumber);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while disabling employee with EmployeeNumber {EmployeeNumber}.", employeeNumber);
            return false;
        }
    }

    public async Task<EmployeeResponseDTO?> GetByEmployeeNumberAsync(string employeeNumber)
    {
        var employee = await _employeeRepository.GetByEmployeeNumberAsync(employeeNumber);
        if (employee == null)
        {
            _logger.LogWarning("No employee found with EmployeeNumber: {EmployeeNumber}", employeeNumber);
            return null;
        }

        return _mapper.Map<EmployeeResponseDTO>(employee);
    }

    public async Task<bool> EnableEmployeeAsync(string employeeNumber)
    {
        var employee = await _employeeRepository.GetByEmployeeNumberAsync(employeeNumber);

        if (employee == null)
        {
            _logger.LogWarning("Employee with employee number {EmployeeNumber} not found", employeeNumber);
            return false;
        }

        if (employee.Status == EmployeeStatus.Active)
        {
            _logger.LogWarning("Employee with employee number {EmployeeNumber} is already active", employeeNumber);
            return false;
        }

        employee.Status = EmployeeStatus.Active;

        try
        {
            await _employeeRepository.UpdateAsync(employee);
            _logger.LogInformation("Employee with employee number {EmployeeNumber} has been successfully enabled.", employeeNumber);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while activating employee with employee number {EmployeeNumber}", employeeNumber);
            return false;
        }
    }

    public async Task<List<EmployeeResponseDTO>> GetAllActiveEmployeesAsync()
    {
        var employees = await _employeeRepository.GetAllActiveAsync();
        return _mapper.Map<List<EmployeeResponseDTO>>(employees);
    }
}
