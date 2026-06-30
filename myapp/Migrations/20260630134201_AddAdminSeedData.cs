using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace myapp.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "id", "CreatedDate", "Email", "PasswordHash", "Role", "UpdatedDate", "Username" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@company.com", "AQAAAAIAAYagAAAAEPNoeIrircn+hscrRUCR+3YlB/k19tLoDJP3lf8JnNpFLaJxzS8OBobVzxUs6sGyvQ==", "Admin", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "super_admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "id",
                keyValue: 1);
        }
    }
}
