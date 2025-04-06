
using EmployeeOnboard.Application.DTOs;

namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
  public interface IUserService
    {
        Task<bool> DisableEmployeeAsync(string employeeNumber);
        Task<bool> EnableEmployeeAsync(string employeeNumber);
        Task<EmployeeResponseDTO?> GetByEmployeeNumberAsync(string employeeNumber);
        Task<List<EmployeeResponseDTO>> GetAllActiveEmployeesAsync();
        Task<EmployeeResponseDTO?> GetEmployeeByEmailAsync(string email);

    }
}
