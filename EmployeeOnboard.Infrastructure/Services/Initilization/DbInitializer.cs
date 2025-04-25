using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Domain.Enums;
using EmployeeOnboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmployeeOnboard.Infrastructure.Services.Initilization;

public class DbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(ApplicationDbContext context, ILogger<DbInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedSuperAdminAsync()
    {
        const string email = "superadmin@company.com";
        var exists = await _context.Employees.AnyAsync(e => e.Email == email);

        if (exists)
        {
            _logger.LogInformation("ℹ️ SuperAdmin already exists.");
            return;
        }

        var superAdmin = new Employee
        {
            Id = Guid.Parse("e7d93a90-78e4-4b0f-bc93-1f78b91d6a52"),
            FirstName = "Super",
            MiddleName = "",
            LastName = "Admin",
            Email = email,
            EmployeeNumber = "SUPERADMIN01",
            Password = BCrypt.Net.BCrypt.HashPassword("SuperAdmin@123"),
            Role = "SuperAdmin",
            CreatedAt = DateTime.UtcNow,
            Status = EmployeeStatus.Active,
            //RefreshToken = null,
            //RefreshTokenExpiryTime = null
        };

        _context.Employees.Add(superAdmin);
        await _context.SaveChangesAsync();

        _logger.LogInformation("✅ SuperAdmin seeded successfully.");
    }
}
