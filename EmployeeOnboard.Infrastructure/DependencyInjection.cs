using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeOnboard.Application.Interfaces.Services;
using EmployeeOnboard.Infrastructure.Services.Notification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeOnboard.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register NotificationService with INotificationService interface
            services.AddScoped<INotificationService, NotificationService>();

            // Register EmailTemplateService
            services.AddScoped<EmailTemplateService>();

            // Register SmtpClientWrapper for dependency injection
            services.AddTransient<ISmtpClientWrapper>(provider =>
            {
                var smtp = configuration.GetSection("Smtp");

                return new SmtpClientWrapper(
                    smtp["Host"],
                    int.Parse(smtp["Port"]),
                    smtp["UserName"],
                    smtp["Password"],
                    bool.Parse(smtp["EnableSsl"])
                );
            });

            return services;
        }
    }
}
    //public static class DependencyInjection
    //{
    //    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    //    {
    //        services.AddScoped<INotificationService, NotificationService>();

    //        //Register EmailTemplateService
    //        services.AddScoped<EmailTemplateService>();

    //        return services;
    //    }

    //}



