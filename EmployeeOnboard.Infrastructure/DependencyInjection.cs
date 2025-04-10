﻿using EmployeeOnboard.Application.Interfaces;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Infrastructure.Repositories;
using EmployeeOnboard.Infrastructure.Services;
using EmployeeOnboard.Infrastructure.Services.Notification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeOnboard.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Services
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IRegisterService, RegisterService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, LoginService>();
            services.AddScoped<ILogoutService, LogoutService>();

            // Register EmailTemplateService
            services.AddScoped<EmailTemplateService>();

            //services.AddHostedService<BackgroundEmailRetryWorker>();
            //services.AddScoped<EmailRetryService>();


            //register repositories
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
           services.AddScoped<IEmailLogRepository, EmailLogRepository>();

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



