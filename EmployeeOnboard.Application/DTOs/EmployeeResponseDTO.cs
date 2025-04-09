
namespace EmployeeOnboard.Application.DTOs
{
    public class EmployeeResponseDTO
    {
        public string EmployeeNumber { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string Status { get; set; } = default!;
    }
}
