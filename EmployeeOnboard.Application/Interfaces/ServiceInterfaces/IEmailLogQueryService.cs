
using EmployeeOnboard.Application.DTOs;

namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
    public interface IEmailLogQueryService
    {
        Task<List<FailedEmailDTO>> GetFailedEmailsAsync();
    }
}
