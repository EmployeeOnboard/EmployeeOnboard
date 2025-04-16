using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using Microsoft.Extensions.Logging;


namespace EmployeeOnboard.Infrastructure.Services
{
    public class UpdateProfileService : IUpdateProfileService
    {
        private readonly ILogger<UpdateProfileService> _logger;
        private readonly IEmployeeRepository _employeeRepository;

        public UpdateProfileService(
            ILogger<UpdateProfileService> logger,
            IEmployeeRepository employeeRepository)
        {
            _logger = logger;
            _employeeRepository = employeeRepository;
        }

        public async Task <string> UpdateProfileAsync(UpdateProfileDTO dto, string userId, string role)
        {
            //retrieve the user
            var user = await _employeeRepository.GetByEmailAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with Email {userId} not found");
                return "user not found";
            }

            //update fields based on roles 

            if (role == "Employee")
            {
                user.PhoneNumber = dto.PhoneNumber;
                user.AltPhoneNumber = dto.AltPhoneNumber;
                user.Address = dto.Address;
                user.ProfileImgUrl = dto.ProfileImgUrl;
            }

            else if (role == "Admin" || role == "SuperAdmin")
            {
                user.FirstName = dto.FirstName;
                user.Email = dto.Email;
                user.Role = dto.Role;
                user.PhoneNumber = dto.PhoneNumber;
                user.AltPhoneNumber = dto.AltPhoneNumber;
                user.Address = dto.Address;
                user.ProfileImgUrl = dto.ProfileImgUrl;
            }

            //save changes 
            await _employeeRepository.UpdateAsync(user);

            //return success message 
            return "Profile updated successfully";

        }

    }
}

   


