using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolSystem.Backend.Migrations
{
    /// <inheritdoc />
    public partial class MultiTenantRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlatformName = table.Column<string>(type: "text", nullable: false),
                    SupportEmail = table.Column<string>(type: "text", nullable: false),
                    EnablePayments = table.Column<bool>(type: "boolean", nullable: false),
                    EnableSmsNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    EnableEmailInvitations = table.Column<bool>(type: "boolean", nullable: false),
                    MaxTenants = table.Column<int>(type: "integer", nullable: false),
                    MaxUsersPerTenant = table.Column<int>(type: "integer", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemSettings");
        }
    }
}
