using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Shared;

namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
    public interface IUpdateProfileService
    {
        Task<Result<UpdatedUserDTO>> UpdateProfileAsync(UpdateProfileDTO dto, string userId, string role);

    }
}
