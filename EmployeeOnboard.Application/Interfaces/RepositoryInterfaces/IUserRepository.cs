using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeOnboard.Domain.Entities;  

namespace EmployeeOnboard.Application.Interfaces.RepositoryInterfaces
{
   public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> UpdatePasswordAsync(User user);
        
    }
}
   