
//using EmployeeOnboard.Domain.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace EmployeeOnboard.Infrastructure.Config;

//public class EmployeeConfig : IEntityTypeConfiguration<Employee>
//{
//    public void Configure(EntityTypeBuilder<Employee> builder)
//    {
//        builder.HasKey(e => e.Id);
//        builder.Property(e => e.EmployeeNumber).IsRequired().HasMaxLength(20);
//        builder.Property(e => e.FullName).IsRequired().HasMaxLength(100);
//        builder.Property(e => e.Email).IsRequired().HasMaxLength(100);
//        builder.HasIndex(e => e.Email).IsUnique();
//        builder.Property(e => e.Password).IsRequired(); //check on this more
//        builder.Property(e => e.IsPasswordChanged).IsRequired(); 
//        builder.Property(e => e.Role).IsRequired().HasMaxLength(50);
//        builder.Property(e => e.Status).IsRequired();
//        builder.Property(e => e.CreatedAt).IsRequired();
//    }
//}
