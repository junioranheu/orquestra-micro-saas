using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AltCompanyAddPlanType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Modules",
                table: "CompanyInvoices");

            migrationBuilder.DropColumn(
                name: "CompanyModules",
                table: "Companies");

            migrationBuilder.AddColumn<int>(
                name: "PlanType",
                table: "CompanyInvoices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlanType",
                table: "Companies",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlanType",
                table: "CompanyInvoices");

            migrationBuilder.DropColumn(
                name: "PlanType",
                table: "Companies");

            migrationBuilder.AddColumn<int[]>(
                name: "Modules",
                table: "CompanyInvoices",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<int[]>(
                name: "CompanyModules",
                table: "Companies",
                type: "integer[]",
                nullable: true);
        }
    }
}
