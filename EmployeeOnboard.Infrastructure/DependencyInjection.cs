﻿using EmployeeOnboard.Application.Interfaces;
using EmployeeOnboard.Application.Interfaces.RepositoryInterfaces;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Application.Interfaces.UOW;
using EmployeeOnboard.Application.Shared;
using EmployeeOnboard.Infrastructure.Repositories;
using EmployeeOnboard.Infrastructure.Services;
using EmployeeOnboard.Infrastructure.Services.BackgroundJobs;
using EmployeeOnboard.Infrastructure.Services.Employees;
using EmployeeOnboard.Infrastructure.Services.Initilization;
using EmployeeOnboard.Infrastructure.Services.Notification;
using EmployeeOnboard.Infrastructure.Services.PasswordManagementService;
using EmployeeOnboard.Infrastructure.UOW;
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
            services.AddScoped<IUpdateProfileService, UpdateProfileService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IChangePassword, ChangePasswordService>();
            services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
            services.AddScoped<IForgotPasswordTokenRepository, ForgotPasswordTokenRepository>();
            services.AddScoped<IEmailRetryProcessor, EmailRetryProcessor>();
            services.AddScoped<IEmailLogQueryService, EmailLogQueryService>();
            services.AddScoped<EmailTemplateService>();
            services.AddScoped<IEmailRetryService, EmailRetryService>();
            services.AddScoped<IEmailLogQueryService, EmailLogQueryService>();

            //register repositories
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmailLogRepository, EmailLogRepository>();

            // register the initializer class
            services.AddScoped<DbInitializer>();

            //Hosted Services
            services.AddHostedService<EmailRetryBackgroundService>();

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



