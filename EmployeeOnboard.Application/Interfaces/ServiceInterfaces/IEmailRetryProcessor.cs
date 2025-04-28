
namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
    public interface IEmailRetryProcessor
    {
        Task<string> RetryEmailAsync(string recipientEmail);
    }
}
