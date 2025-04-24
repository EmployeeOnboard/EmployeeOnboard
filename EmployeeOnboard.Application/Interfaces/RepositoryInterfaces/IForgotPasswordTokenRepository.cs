using EmployeeOnboard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Application.Interfaces.RepositoryInterfaces
{
    public interface IForgotPasswordTokenRepository
    {
        Task<ForgotPasswordToken> GetByEmployeeIdAsync(Guid employeeId);
        Task AddAsync(ForgotPasswordToken token);
        Task UpdateAsync(ForgotPasswordToken token);

    }
}
