using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Logging;

namespace EmployeeOnboard.Infrastructure.Services; 
public class ResetPasswordService : IResetPasswordService
{
    private readonly IResetPasswordRepository _resetPasswordRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ResetPasswordService> _logger;

    public ResetPasswordService(IResetPasswordRepository resetPasswordRepository, IUserRepository userRepository, ILogger<ResetPasswordService> logger)
    {
        _resetPasswordRepository = resetPasswordRepository ?? throw new ArgumentNullException(nameof(resetPasswordRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordDTO resetPasswordRequest)
    {
        //if (resetPasswordRequest == null)
        //{
        //    _logger.LogWarning("Reset password request is null.");
        //    return new ResetPasswordResponse { IsSuccess = false, Message = "Invalidrequest." };
        //}
        var user = await _userRepository.GetUserByEmailAsync(resetPasswordRequest.Email);

        if (user == null)
        {
            _logger.LogWarning("User not found for email: {Email}", resetPasswordRequest.Email);
            return new ResetPasswordResponse { IsSuccess = false, Message = "User not found." };
        }
  
        //if (resetPasswordRequest.CurrentPassword != user.PasswordHash )
        //{
        //    _logger.LogWarning("Current password is incorrect for email: {Email}", resetPasswordRequest.Email);
        //    return new ResetPasswordResponse { IsSuccess = false, Message = "Current Password is incorrect." };
        //}

         
        if (resetPasswordRequest.NewPassword != resetPasswordRequest.ConfirmPassword)
        {
            _logger.LogWarning("Password and Confirm Password do not match for email: {Email}", resetPasswordRequest.Email);
            return new ResetPasswordResponse { IsSuccess = false, Message = "Passwords do not match." };
        }

        

        // Verify if the provided current password matches the stored password hash
        if (!BCrypt.Net.BCrypt.Verify(resetPasswordRequest.CurrentPassword, user.PasswordHash)) // Fix: verify against current password, not the hashed value
        {
            _logger.LogWarning("Current password is incorrect for email: {Email}", resetPasswordRequest.Email);
            return new ResetPasswordResponse { IsSuccess = false, Message = "Current password is incorrect." };
        }


        // Hash the new password before updating it
        string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordRequest.NewPassword);
        bool isUpdated = await _resetPasswordRepository.UpdatePasswordAsync(resetPasswordRequest.Email, newPasswordHash);

        if (!isUpdated)
        {
            _logger.LogError("Failed to update password for email: {Email}", resetPasswordRequest.Email);
            return new ResetPasswordResponse { IsSuccess = false, Message = "Password reset failed. Please try again." };
        }

        _logger.LogInformation("Password successfully reset for email: {Email}", resetPasswordRequest.Email);
        return new ResetPasswordResponse { IsSuccess = true, Message = "Password reset successfully." };
    }
}
