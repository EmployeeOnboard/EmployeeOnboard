//using EmployeeOnboard.Infrastructure.Data;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.EntityFrameworkCore;
//using EmployeeOnboard.Api;
//using Microsoft.Extensions.DependencyInjection;

//namespace EmployeeOnboard.Tests.IntegrationTests;

//internal class EmployeeOnboardingApiFactory : WebApplicationFactory<Program> // made this internal since program.cs is internal by default and it cant be accessed by a public class
//{
//    protected override void ConfigureWebHost(IWebHostBuilder builder)
//    {
//        builder.ConfigureServices(services =>
//        {
//            // This removes the existing DbContext registration
//            var descriptor = services.SingleOrDefault(
//                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

//            if (descriptor != null)
//            {
//                services.Remove(descriptor);
//            }

//            // this adds an in-memory database for testing
//            services.AddDbContext<ApplicationDbContext>(options =>
//            {
//                options.UseInMemoryDatabase("TestDb");
//            });

//            var sp = services.BuildServiceProvider(); // rebuilds the service provider to apply changes

//            using (var scope = sp.CreateScope()) // ensures that the db is created
//            {
//                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//                db.Database.EnsureCreated();
//            }
//        });
//    }
//}
