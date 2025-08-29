using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCompanyUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccountVerified",
                table: "CompanyUsers");

            migrationBuilder.DropColumn(
                name: "IsAccountVerified",
                table: "Companies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccountVerified",
                table: "CompanyUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAccountVerified",
                table: "Companies",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
