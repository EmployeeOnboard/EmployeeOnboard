using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Domain.Entities;

namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
    public interface IRegisterService
    {
        Task<(bool IsSuccess, string Message)> RegisterEmployeeAsync(Employee request);
    }
}