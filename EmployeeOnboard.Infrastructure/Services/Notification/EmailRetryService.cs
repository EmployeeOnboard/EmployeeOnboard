using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;

namespace EmployeeOnboard.Infrastructure.Services.Notification;

public class EmailRetryService : IEmailRetryService
{
    private readonly IEmailRetryProcessor _emailRetryProcessor;

    public EmailRetryService(IEmailRetryProcessor emailRetryProcessor)
    {
        _emailRetryProcessor = emailRetryProcessor;
    }

    public async Task<List<string>> RetrySelectedEmailsAsync(List<string> recipientEmails)
    {
        var results = new List<string>();

        foreach (var email in recipientEmails)
        {
            var result = await _emailRetryProcessor.RetryEmailAsync(email);
            results.Add(result);
        }

        return results;
    }
}
