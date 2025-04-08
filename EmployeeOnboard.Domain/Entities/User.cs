using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }  // Primary Key
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        // Make NewPassword and ConfirmPassword nullable
       // public string? NewPassword { get; set; }
        //public string? ConfirmPassword { get; set;          

        // Make IsSuccess nullable
        //public bool? IsSuccess { get; set; }  // Nullable boolean

        // Make Message nullable
        //public string? Message { get; set; } = null;  // Nullable string    
    }
}
         