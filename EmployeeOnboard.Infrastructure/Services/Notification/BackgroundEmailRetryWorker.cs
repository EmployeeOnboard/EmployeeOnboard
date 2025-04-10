
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;

//namespace EmployeeOnboard.Infrastructure.Services.Notification
//{
//    public class BackgroundEmailRetryWorker : BackgroundService
//    {
//        private readonly IServiceProvider _serviceProvider;
//        private readonly ILogger<BackgroundEmailRetryWorker> _logger;

//        public BackgroundEmailRetryWorker(IServiceProvider serviceProvider, ILogger<BackgroundEmailRetryWorker> logger)
//        {
//            _serviceProvider = serviceProvider;
//            _logger = logger;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            _logger.LogInformation("Email Retry Worker started.");

//            while (!stoppingToken.IsCancellationRequested)
//            {
//                using var scope = _serviceProvider.CreateScope();
//                var retryService = scope.ServiceProvider.GetRequiredService<EmailRetryService>();

//                try
//                {
//                    await retryService.RetryFailedEmailAsync(stoppingToken);
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Email retry task failed.");
//                }

//                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Wait 5 minutes before retrying
//            }
//        }
//    }
//}
