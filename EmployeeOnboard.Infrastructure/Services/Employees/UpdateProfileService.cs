using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using Microsoft.Extensions.Logging;
using EmployeeOnboard.Application.Interfaces.UOW;
using EmployeeOnboard.Infrastructure.UOW;
using EmployeeOnboard.Infrastructure.Repositories;
using EmployeeOnboard.Application.Shared;


namespace EmployeeOnboard.Infrastructure.Services.Employees
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

        public async Task<Result<UpdatedUserDTO>> UpdateProfileAsync(UpdateProfileDTO dto, string userId, string role)
        {
            //retrieve the user
            var user = await _unitOfWork.Employee.GetByEmailAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with Email {userId} not found");
                return Result<UpdatedUserDTO>.Failure("User not found");
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
                user.MiddleName = dto.MiddleName;
                user.LastName = dto.LastName;
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
            {
                var UpdatedUserDto = new UpdatedUserDTO
                {
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role,
                    PhoneNumber = user.PhoneNumber,
                    AltPhoneNumber = user.AltPhoneNumber,
                    Address = user.Address,
                    ProfileImgUrl = user.ProfileImgUrl
                };

                return Result<UpdatedUserDTO>.Success(UpdatedUserDto, "Profile updated successfully.");
            }
            else
            {
                return Result<UpdatedUserDTO>.Failure("No changes were saved.");
            }

        }
    }
}







