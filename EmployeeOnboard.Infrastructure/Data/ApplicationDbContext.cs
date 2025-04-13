
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
    public DbSet<EmailLog> EmailLogs { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<Employee>()
       .HasIndex(e => e.EmployeeNumber)
       .IsUnique();           // this makes sure that the employee number generated is unique for every employee. 

    }
}
