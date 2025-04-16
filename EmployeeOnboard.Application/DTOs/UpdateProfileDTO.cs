using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Application.DTOs
{
    public class UpdateProfileDTO
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public string AltPhoneNumber { get; set; }

        public string Address { get; set; }
        public string? ProfileImgUrl { get; set; }


    }
}
