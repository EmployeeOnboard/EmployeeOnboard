using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using Microsoft.Extensions.Logging;
using EmployeeOnboard.Application.Interfaces.UOW;
using EmployeeOnboard.Infrastructure.UOW;
using EmployeeOnboard.Infrastructure.Repositories;


namespace EmployeeOnboard.Infrastructure.Services
{
    public class UpdateProfileService : IUpdateProfileService
    {
        private readonly ILogger<UpdateProfileService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProfileService(
            ILogger<UpdateProfileService> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task <string> UpdateProfileAsync(UpdateProfileDTO dto, string userId, string role)
        {
            //retrieve the user
            var user = await _unitOfWork.Employee.GetByEmailAsync(userId);
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
            var rowsAffected = await _unitOfWork.SaveChangesAsync();

            //return success message 
            if (rowsAffected > 0)

                return /*Result.Success*/("Profile updated successfully.");

            return /*Result.Failure*/("No changes were saved.");
        }

    }
}

   


