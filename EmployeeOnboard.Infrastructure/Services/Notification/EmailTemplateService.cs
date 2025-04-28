

using System.Text.Json;
using EmployeeOnboard.Application.Interfaces.ServiceInterfaces;
using EmployeeOnboard.Domain.Enums;
using EmployeeOnboard.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static System.Collections.Specialized.BitVector32;



namespace EmployeeOnboard.Infrastructure.Services.Notification;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailTemplateService> _logger;
    private readonly string _templatesPath;


    public EmailTemplateService(IConfiguration configuration, ILogger<EmailTemplateService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _templatesPath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "EmailTemplates.json");
    }


    //Get a specific template key
    public EmailTemplateModel GetTemplate(EmailTemplateType templateType)

    {

        string templateKey = templateType.ToString(); // ✅ clean and clear

        // load the templates section from configuration
        var templates = _configuration.GetSection($"EmailTemplates:{templateKey}");

        if (!templates.Exists())
        {
            _logger.LogError($"No email template found for key '{templateKey}' in configuration.");
            throw new KeyNotFoundException($"No email template found for key: {templateKey}");
        }

        //return the template from configuration

        return new EmailTemplateModel
        {
            Subject = templates["Subject"] ?? "",

            Body = templates["Body"] ?? ""
        };


    }

    public (string Subject, string Body) ReplacePlaceholders(string templateSubject, string templateBody, Dictionary<string, string> placeholders)
    {
        if (placeholders == null || placeholders.Count == 0)
        {
            Console.WriteLine("No placeholders provided.");
            return (templateSubject, templateBody);
        }


        foreach (var placeholder in placeholders)
        {
            Console.WriteLine($" Replacing {placeholder.Key} with {placeholder.Value}");
            templateSubject = templateSubject.Replace($"{{{placeholder.Key}}}", placeholder.Value);
        }

        foreach (var placeholder in placeholders)
        {
            Console.WriteLine($"Replacing {placeholder.Key} with {placeholder.Value}");
            templateBody = templateBody.Replace($"{{{placeholder.Key}}}", placeholder.Value);
        }

        Console.WriteLine("Final processed template:");
        Console.WriteLine(templateBody);

        Console.WriteLine("Final processed template:");
        Console.WriteLine(templateBody);

        return (templateSubject, templateBody);
    }


}
