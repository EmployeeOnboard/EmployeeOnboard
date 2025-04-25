using EmployeeOnboard.Application.DTOs.PasswordManagementDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
    public interface IForgotPasswordService
    {
        Task ForgotPasswordAsync(string email);
        Task ResetPasswordAsync(ResetPasswordRequestDTO request);


    }
}
