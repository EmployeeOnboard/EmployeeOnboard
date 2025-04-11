using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
    public interface IAuthService
    {
        Task<(bool IsSuccess, string Message)> SendPasswordResetTokenAsync(string email);
    }
}
