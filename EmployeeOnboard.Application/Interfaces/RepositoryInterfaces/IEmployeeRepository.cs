
using EmployeeOnboard.Domain.Entities;

namespace EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id);
    Task<Employee?> GetByEmailAsync(string email);
    Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber);
    Task<List<Employee>> GetAllAsync();
    Task AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(Employee employee);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByEmployeeNumberAsync(string employeeNumber);
}
