using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Users",
                table: "Schedules",
                newName: "UsersIds");

            migrationBuilder.AddColumn<bool>(
                name: "IsRestrictForSpecificUsers",
                table: "Schedules",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRestrictForSpecificUsers",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "UsersIds",
                table: "Schedules",
                newName: "Users");
        }
    }
}
