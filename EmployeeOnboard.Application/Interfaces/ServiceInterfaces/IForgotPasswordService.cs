using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
    public interface IForgotPasswordService
    {
        Task HandleAsync(ForgotPasswordRequest request);
    }
}
