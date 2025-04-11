using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
        Task SendResetPasswordEmail(string toEmail, string resetLink);
    }
}
