
namespace EmployeeOnboard.Application.DTOs;

public class FailedEmailDTO
{
    public string RecipientEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastRetriedAt { get; set; }
}
