
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Domain.Enums;
using EmployeeOnboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboard.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Employee employee)
    {
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Employees.AnyAsync(e => e.Email == email);
    }

    public async Task<bool> ExistsByEmployeeNumberAsync(string employeeNumber)
    {
        return await _context.Employees.AnyAsync(e => e.EmployeeNumber == employeeNumber);
    }

    public async Task<List<Employee>> GetAllActiveAsync()
    {
        return await _context.Employees
            .Where(e => e.Status == EmployeeStatus.Active)
            .ToListAsync();
    }

    public async Task<Employee?> GetByEmailAsync(string email)
    {
        return await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
    }

    public async Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber)
    {
       return await _context.Employees.FirstOrDefaultAsync(e=>e.EmployeeNumber == employeeNumber);
    }

    public async Task UpdateAsync(Employee employee)
    {
        var existingEmployee = await _context.Employees.FindAsync(employee.Id);

        if (existingEmployee?.Role == "SuperAdmin" && employee.Role != "SuperAdmin")
        {
            throw new InvalidOperationException("Super Admin role cannot be changed."); //this will prevent unauthorized update of the superadmin 
        }

        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
    }
}
