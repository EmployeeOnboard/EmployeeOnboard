using EmployeeOnboard.Application.DTOs;

namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
    public interface IRegisterService
    {
        Task<(bool IsSuccess, string Message)> RegisterEmployeeAsync(RegisterEmployeeDTO dto);
    }
}