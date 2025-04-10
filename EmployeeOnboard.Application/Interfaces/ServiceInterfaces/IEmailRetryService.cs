
namespace EmployeeOnboard.Application.Interfaces.ServiceInterfaces
{
    public interface IEmailRetryService
    {
        Task RetryFailedEmailAsync(CancellationToken cancellationToken);
    }
}