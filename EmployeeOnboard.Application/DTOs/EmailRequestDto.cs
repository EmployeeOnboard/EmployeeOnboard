using System.ComponentModel.DataAnnotations;

namespace EmployeeOnboard.Application.DTOs
{

    public class EmailRequestDto
    {

        [Required(ErrorMessage = "Recipient email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string To { get; set; }

        [Required(ErrorMessage = "Template key is required.")]
        public string TemplateKey { get; set; }
        public Dictionary<string, string> Placeholders { get; set; } = new();


    }
}
