
using EmployeeOnboard.Application.DTOs;
using FluentValidation;
using System.Text.RegularExpressions;

namespace EmployeeOnboard.Application.Validators
{
    public class RegisterEmployeeValidator: AbstractValidator<RegisterEmployeeDTO>
    {
        private bool BeValidKenyanPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^254\d{9}$");
        }

        public RegisterEmployeeValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First Name is required")
                .MaximumLength(50)
                .WithMessage("First Name cannot exceed 50 characters");
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last Name is required")
                .MaximumLength(50)
                .WithMessage("Last Name cannot exceed 50 characters");
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone Number is required")
                .Must(BeValidKenyanPhoneNumber)
                .WithMessage("Invalid phone number format.");
            RuleFor(x => x.Role)
                 .NotEmpty()
                 .WithMessage("Role is required");
        }
    }
}
