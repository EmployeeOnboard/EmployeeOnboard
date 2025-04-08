using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Infrastructure.Services
{
    public  class EmailService : IEmailService
    {
        public async Task SendResetPasswordEmail(string toEmail, string resetLink)
        {
            // Your logic for sending email
            await Task.CompletedTask; // placeholder
        }
    }
}
