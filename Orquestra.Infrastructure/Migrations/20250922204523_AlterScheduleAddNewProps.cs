using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterScheduleAddNewProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountReceived",
                table: "Schedules",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomTitle",
                table: "Schedules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomUrl",
                table: "Schedules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observation",
                table: "Schedules",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountReceived",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "CustomTitle",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "CustomUrl",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "Observation",
                table: "Schedules");
        }
    }
}
