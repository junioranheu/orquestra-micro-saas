using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterScheduleChangeDateProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Schedules",
                newName: "DateStart");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEnd",
                table: "Schedules",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateEnd",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "DateStart",
                table: "Schedules",
                newName: "Date");

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Schedules",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
