
using EmployeeOnboard.Domain.Entities;

namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
    public interface IEmailRetryService
    {
        Task<List<string>> RetrySelectedEmailsAsync(List<string> recipientEmails);
    }
}