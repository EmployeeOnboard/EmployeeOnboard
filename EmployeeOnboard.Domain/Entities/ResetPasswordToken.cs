using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Domain.Entities
{
    public class ResetPasswordToken
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Email { get; private set; }
        public string Token { get; private set; }
        public DateTime ExpiryDate { get; private set; }
        public bool IsUsed { get; private set; }

        public ResetPasswordToken(string email, string token, DateTime expiryDate)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.");
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Token is required.");
            if (expiryDate <= DateTime.UtcNow) throw new ArgumentException("Expiry date must be in the future.");

            Email = email;
            Token = token;
            ExpiryDate = expiryDate;
            IsUsed = false;
        }

        public void MarkAsUsed()
        {
            if (IsUsed) throw new InvalidOperationException("Token has already been used.");
            IsUsed = true;
        }

        public bool IsValid()
        {
            return !IsUsed && ExpiryDate > DateTime.UtcNow;
        } 
    }
}
