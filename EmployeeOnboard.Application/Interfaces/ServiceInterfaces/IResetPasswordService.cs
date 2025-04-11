using EmployeeOnboard.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{ 
   public interface IResetPasswordService
    {
        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordDTO resetPasswordRequest);

    }
} 