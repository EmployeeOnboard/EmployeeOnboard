
namespace EmployeeOnboard.Application.DTOs
{ 
    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
         
    }   
}
