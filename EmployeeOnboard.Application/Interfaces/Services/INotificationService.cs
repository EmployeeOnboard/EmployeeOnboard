using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeOnboard.Application.DTOs;

namespace EmployeeOnboard.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task SendEmailAsync(EmailRequestDto request);
    }
}
