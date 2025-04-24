using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeOnboard.Domain.Entities
{
    public class ForgotPasswordToken
    {
        [Key]
        public Guid Id { get; set; }

        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
    }
}
