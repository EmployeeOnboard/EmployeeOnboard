using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Application.Interfaces.UOW;
using EmployeeOnboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboard.Infrastructure.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IEmployeeRepository Employee { get; }


        public UnitOfWork(ApplicationDbContext context, IEmployeeRepository employee)
        {
            _context = context;
            Employee = employee;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
