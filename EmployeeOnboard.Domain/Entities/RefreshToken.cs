

namespace EmployeeOnboard.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        // Foreign key to Employee
        public Guid EmployeeId { get; set; }  
        public Employee Employee { get; set; }
    }
}


