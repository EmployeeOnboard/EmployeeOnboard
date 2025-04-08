using Microsoft.EntityFrameworkCore;
using EmployeeOnboard.Domain.Entities;

namespace EmployeeOnboard.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define your DbSets (tables)
        //public DbSet<Users> Users { get; set; }
        public virtual DbSet<Users> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Users>();

        }
    }
}
