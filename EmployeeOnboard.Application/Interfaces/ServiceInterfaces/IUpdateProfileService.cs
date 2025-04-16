using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeOnboard.Application.DTOs;

namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
    public interface IUpdateProfileService
    {
        Task<string> UpdateProfileAsync(UpdateProfileDTO dto, string userId, string role);

    }
}
