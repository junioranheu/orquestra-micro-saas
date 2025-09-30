using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AltLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LogType",
                table: "Logs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogType",
                table: "Logs");
        }
    }
}
