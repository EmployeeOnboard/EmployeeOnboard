
using EmployeeOnboard.Domain.Enums;
using EmployeeOnboard.Domain.Models;

namespace EmployeeOnboard.Infrastructure.Services.Notification
{
    public interface IEmailTemplateService
    {
        EmailTemplateModel GetTemplate(EmailTemplateType templateType);
        (string Subject, string Body) ReplacePlaceholders(string templateSubject, string templateBody, Dictionary<string, string> placeholders);
    }
}