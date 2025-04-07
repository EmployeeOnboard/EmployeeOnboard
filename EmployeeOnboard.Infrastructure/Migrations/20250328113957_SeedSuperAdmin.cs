using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeOnboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedSuperAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CreatedAt", "Email", "EmployeeNumber", "FullName", "IsPasswordChanged", "JobRole", "Password", "Status" },
                values: new object[] { new Guid("7da16b9d-bf75-47aa-bc4b-c3e848df3bf0"), new DateTime(2025, 3, 28, 11, 39, 47, 579, DateTimeKind.Utc).AddTicks(3504), "superadmin@company.com", "SUPERADMIN01", "Super Admin", false, "SuperAdmin", "$2a$11$L/oneTmlXoGpEgOgITSiqeJD/.o3lJ5LnKFzC5zm07bRj93cknIFq", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("7da16b9d-bf75-47aa-bc4b-c3e848df3bf0"));
        }
    }
}
