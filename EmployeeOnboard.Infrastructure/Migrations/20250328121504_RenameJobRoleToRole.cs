using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeOnboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameJobRoleToRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JobRole",
                table: "Employees",
                newName: "Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Employees",
                newName: "JobRole");
        }
    }
}
