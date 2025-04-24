
namespace EmployeeOnboard.Application.DTOs;

public class RegisterEmployeeDTO
{
    public  required string FirstName { get; set; }
    public string MiddleName { get; set; }
    public required string LastName { get; set; }

    //public string FullName { get; set; }
    public required string PhoneNumber { get; set; }
    public string AltPhoneNumber { get; set; }
    public required string Email { get; set; }
    public string Address { get; set; }
    public required string Role { get; set; }
}



