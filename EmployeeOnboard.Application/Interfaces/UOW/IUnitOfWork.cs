using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;

namespace EmployeeOnboard.Application.Interfaces.UOW
{
    public interface IUnitOfWork
    {
        IEmployeeRepository Employee { get; }

        Task<int> SaveChangesAsync(); //commits all changes as a single transcation

    }
}
