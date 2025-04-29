using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Domain.Entities;
using System.Threading.Tasks;
using EmployeeOnboard.Application.DTOs.PasswordManagementDTO;
using BCrypt.Net;

namespace EmployeeOnboard.Infrastructure.Services.PasswordManagementService
{
    public class ChangePasswordService : IChangePassword
    {
        private readonly IEmployeeRepository _employeeRepository;

        public ChangePasswordService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task ChangePasswordAsync(ChangePasswordDTO dto)
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
                throw new Exception("New password and confirmation do not match.");

            var user = await _employeeRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("User not found.");

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.Password);
            if (!isPasswordValid)
                throw new Exception("Current password is incorrect.");

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _employeeRepository.UpdateAsync(user);
        }
    }
}
 