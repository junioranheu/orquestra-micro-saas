using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AltEntityClientFollowUpAddPropScheduleId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScheduleId",
                table: "ClientsFollowUps",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ClientsFollowUps_ScheduleId",
                table: "ClientsFollowUps",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientsFollowUps_Schedules_ScheduleId",
                table: "ClientsFollowUps",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "ScheduleId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientsFollowUps_Schedules_ScheduleId",
                table: "ClientsFollowUps");

            migrationBuilder.DropIndex(
                name: "IX_ClientsFollowUps_ScheduleId",
                table: "ClientsFollowUps");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "ClientsFollowUps");
        }
    }
}
