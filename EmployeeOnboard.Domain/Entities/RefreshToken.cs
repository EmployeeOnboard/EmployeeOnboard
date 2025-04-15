using EmployeeOnboard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }

        // Foreign key to Employee
        public Guid EmployeeId { get; set; }    // <--- Make sure this is Guid, not int
        public Employee Employee { get; set; }
    }
}


