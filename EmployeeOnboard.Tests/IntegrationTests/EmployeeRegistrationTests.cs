
//using FluentAssertions;
//using System.Net.Http.Json;

//namespace EmployeeOnboard.Tests.IntegrationTests;

//public class EmployeeRegistrationTests : IClassFixture<EmployeeOnboardingApiFactory>
//{
//    private readonly HttpClient _client;

//    public EmployeeRegistrationTests(EmployeeOnboardingApiFactory factory)
//    {
//        _client = factory.CreateClient();
//    }

//    [Fact]
//    public async Task RegisterEmployee_ShouldReturn201Created_WhenValidInput()
//    {
//        // Creating a valid request payload
//        var request = new
//        {
//            Email = "testuser@example.com",
//            FirstName = "John",
//            MiddleName = "Doe",
//            LastName = "Smith",
//            PhoneNumber = "254712345678",
//            Role = "Developer"
//        };

//        // Sending a POST request to the API
//        var response = await _client.PostAsJsonAsync("/api/employees/register", request);

//        // this checks response status
//        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

//        // Verifies the response contains an employee number
//        var content = await response.Content.ReadAsStringAsync();
//        content.Should().Contain("Employee registered successfully. Employee Number:");
//    }
//}
