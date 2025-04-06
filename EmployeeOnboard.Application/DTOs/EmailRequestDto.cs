using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Application.DTOs
{

    public class EmailRequestDto
    {
        public string To { get; set; }
        public string TemplateKey { get; set; }
        public Dictionary<string, string> Placeholders { get; set; } = new();


    }
}
