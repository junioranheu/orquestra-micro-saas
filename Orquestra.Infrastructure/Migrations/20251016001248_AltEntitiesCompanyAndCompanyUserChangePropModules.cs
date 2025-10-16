using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AltEntitiesCompanyAndCompanyUserChangePropModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Modules",
                table: "CompanyUsers",
                newName: "UserModules");

            migrationBuilder.RenameColumn(
                name: "Modules",
                table: "Companies",
                newName: "CompanyModules");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserModules",
                table: "CompanyUsers",
                newName: "Modules");

            migrationBuilder.RenameColumn(
                name: "CompanyModules",
                table: "Companies",
                newName: "Modules");
        }
    }
}
