using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Application.Interfaces.RepositoryInterfaces
{
    public interface IResetPasswordRepository
    {
        
        Task<bool> UpdatePasswordAsync(string email, string newPassword);
        
    }
}
   