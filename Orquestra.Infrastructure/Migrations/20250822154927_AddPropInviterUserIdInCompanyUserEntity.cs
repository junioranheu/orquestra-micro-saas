using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPropInviterUserIdInCompanyUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InviterUserId",
                table: "CompanyUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUsers_InviterUserId",
                table: "CompanyUsers",
                column: "InviterUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyUsers_Users_InviterUserId",
                table: "CompanyUsers",
                column: "InviterUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyUsers_Users_InviterUserId",
                table: "CompanyUsers");

            migrationBuilder.DropIndex(
                name: "IX_CompanyUsers_InviterUserId",
                table: "CompanyUsers");

            migrationBuilder.DropColumn(
                name: "InviterUserId",
                table: "CompanyUsers");
        }
    }
}
