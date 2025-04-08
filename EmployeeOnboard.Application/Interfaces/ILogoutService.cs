using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Application.Interfaces
{
    public interface ILogoutService
    {
        Task<bool> LogoutAsync(Guid userId);
    }
}
