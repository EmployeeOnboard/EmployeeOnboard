
using EmployeeOnboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeOnboard.Infrastructure.Config;

public class EmployeeRoleConfig : IEntityTypeConfiguration<EmployeeRole> 
{
    public void Configure(EntityTypeBuilder<EmployeeRole> builder)
    {
        builder.HasKey(er => new { er.EmployeeId, er.RoleId });
        builder.HasOne(er => er.employee)
            .WithMany(e => e.EmployeeRoles)
            .HasForeignKey(er => er.EmployeeId);
        builder.HasOne(er => er.role)
            .WithMany(r => r.EmployeeRoles)
            .HasForeignKey(er => er.RoleId);
    }
}
