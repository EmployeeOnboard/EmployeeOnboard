using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Application.Interfaces
{
    public interface ILogoutService
    {
        Task<(bool Success, string Message)> LogoutAsync(ClaimsPrincipal user);
    }
}
