
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmployeeOnboard.Infrastructure.Services.BackgroundJobs
{
    public class EmailRetryBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmailRetryBackgroundService> _logger;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _retryInterval;


        public EmailRetryBackgroundService(IServiceProvider serviceProvider, ILogger<EmailRetryBackgroundService> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;

            int retryMinutes = _configuration.GetValue<int>("BackgroundJobs:RetryIntervalMinutes");
            _retryInterval = TimeSpan.FromMinutes(retryMinutes);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email Retry Background Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();

                    var emailLogQueryService = scope.ServiceProvider.GetRequiredService<IEmailLogQueryService>();
                    var retryProcessor = scope.ServiceProvider.GetRequiredService<IEmailRetryProcessor>();

                    var failedEmails = await emailLogQueryService.GetFailedEmailsAsync();

                    if (failedEmails.Any())
                    {
                        _logger.LogInformation("Found {Count} failed emails. Retrying...", failedEmails.Count);

                        foreach (var failedEmail in failedEmails)
                        {
                            var result = await retryProcessor.RetryEmailAsync(failedEmail.RecipientEmail);

                            _logger.LogInformation(result);
                        }
                    }
                    else
                    {
                        _logger.LogInformation("No failed emails to retry at this time.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occured in the EmailRetryBackgroundService.");
                }

                await Task.Delay(_retryInterval, stoppingToken);
            }
        }
    }
}
