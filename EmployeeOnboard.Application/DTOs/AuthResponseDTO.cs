
namespace EmployeeOnboard.Application.DTOs
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        
    }
}
 