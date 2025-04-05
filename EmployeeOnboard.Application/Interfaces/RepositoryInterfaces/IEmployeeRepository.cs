
using EmployeeOnboard.Domain.Entities;

namespace EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByEmailAsync(string email);
    Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber);
    Task<List<Employee>> GetAllActiveAsync();
    Task AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByEmployeeNumberAsync(string employeeNumber);
}
