using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Schedules_ClientId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_CompanyUsers_CompanyId",
                table: "CompanyUsers");

            migrationBuilder.AddColumn<Guid[]>(
                name: "Users",
                table: "Schedules",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_ClientId_CompanyId",
                table: "Schedules",
                columns: new[] { "ClientId", "CompanyId" });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUsers_CompanyId_UserId",
                table: "CompanyUsers",
                columns: new[] { "CompanyId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CPF",
                table: "Clients",
                column: "CPF");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Email",
                table: "Clients",
                column: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Schedules_ClientId_CompanyId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_CompanyUsers_CompanyId_UserId",
                table: "CompanyUsers");

            migrationBuilder.DropIndex(
                name: "IX_Clients_CPF",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_Email",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Users",
                table: "Schedules");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_ClientId",
                table: "Schedules",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUsers_CompanyId",
                table: "CompanyUsers",
                column: "CompanyId");
        }
    }
}
