using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
    public interface IResetPasswordTokenService
    {
        Task<string> GenerateTokenAsync(string email);
        bool ValidateToken(string email, string token);
    }
}
