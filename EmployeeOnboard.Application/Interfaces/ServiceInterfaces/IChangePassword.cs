using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using EmployeeOnboard.Application.DTOs.PasswordManagementDTO;

namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
    public interface IChangePassword
    {
        Task ChangePasswordAsync(ChangePasswordDTO dto);
    }
}