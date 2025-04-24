
using EmployeeOnboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboard.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<EmployeeRole> EmployeeRoles { get; set; }
    public DbSet<ForgotPasswordToken> ForgotPasswordToken { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<Employee>()
        .HasIndex(e => e.EmployeeNumber)
        .IsUnique(); // this makes sure that the employee number generated is unique for every employee.


        modelBuilder.Entity<Employee>()
       .HasOne(e => e.ForgotPasswordToken)
       .WithOne(t => t.Employee)
       .HasForeignKey<ForgotPasswordToken>(t => t.EmployeeId);

        // Seed Employee after
        var superAdminId = Guid.Parse("e7d93a90-78e4-4b0f-bc93-1f78b91d6a52");

        modelBuilder.Entity<Employee>().HasData(new Employee
        {
            Id = superAdminId,
            FirstName = "Super",
            LastName = "Admin",
            Email = "superadmin@company.com",
            EmployeeNumber = "SUPERADMIN01",
            Password = "$2a$11$Hj2Qj7fPKfTrRUzWYV9nNuec7Yl3xjlJYoE7O7E8R0gGJ9B6xNG1q",
            Role = "SuperAdmin",
            CreatedAt = new DateTime(2025, 3, 27, 0, 0, 0, DateTimeKind.Utc),
        });

    }
}
