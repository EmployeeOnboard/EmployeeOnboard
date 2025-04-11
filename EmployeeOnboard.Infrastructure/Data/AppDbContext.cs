//using Microsoft.EntityFrameworkCore;
//using EmployeeOnboard.Domain.Entities;

//namespace EmployeeOnboard.Infrastructure.Data
//{
//    public class AppDbContext : DbContext
//    {
//        public AppDbContext(DbContextOptions<AppDbContext> options)
//            : base(options)
//        {
//        }

//        public DbSet<User> Users { get; set; } // Ensure it's "User", not "Users"

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);

//            // If you have specific configurations, apply them here
//            modelBuilder.Entity<User>(entity =>
//            {
//                entity.HasKey(e => e.Id); // Assuming Id is your primary key
//                entity.Property(e => e.Email).IsRequired(); // Example constraint
//            });
//        }
//    }
//}














using Microsoft.EntityFrameworkCore;
using EmployeeOnboard.Domain.Entities;

namespace EmployeeOnboard.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicitly defining primary key
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
        }
    }
}
