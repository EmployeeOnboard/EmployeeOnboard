using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Application.Validators;
using FluentValidation.TestHelper;

namespace EmployeeOnboard.Tests.UnitTests.Validators;

public class RegisterEmployeeValidatorTest
{
    private readonly RegisterEmployeeValidator _validator = new();

    [Fact]
    public void Should_HaveError_When_EmailIsInvalid() //this validates email
    {
        var dto = new RegisterEmployeeDTO { Email = "invalid-email", FirstName = "John", MiddleName = "Niko", LastName = "Doe", PhoneNumber = "254712345678", Role = "HR" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(e => e.Email);
    }

    [Fact]
    public void Should_HaveError_When_FirstNameIsEmpty() // validates input of required fields
    {
        var dto = new RegisterEmployeeDTO { Email = "test@example.com", FirstName = "", MiddleName = "Njoki", LastName = "Doe", PhoneNumber = "254712345678", Role = "Developer" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(e => e.FirstName);
    }

    [Theory]
    [InlineData("254712345678")] // This is a valid Kenyan number
    [InlineData("254798765432")] // This one too
    public void Should_Validate_Correct_PhoneNumber(string validPhone)
    {
        var dto = new RegisterEmployeeDTO { PhoneNumber = validPhone };
        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("0712345678")]  // Missing country code
    [InlineData("25412345678")] // Too short
    [InlineData("2547123456789")] // Too long
    [InlineData("25471234abc8")] // Contains letters
    [InlineData("123712345678")] // Does not start with 254
    public void Should_Invalidate_Wrong_PhoneNumber(string invalidPhone)
    {
        var dto = new RegisterEmployeeDTO { PhoneNumber = invalidPhone };
        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
              .WithErrorMessage("Invalid phone number format.");
    }
}
