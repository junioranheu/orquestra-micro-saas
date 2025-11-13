using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AltEntityClientFollowUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientFollowUp_Clients_ClientId",
                table: "ClientFollowUp");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientFollowUp_Companies_CompanyId",
                table: "ClientFollowUp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientFollowUp",
                table: "ClientFollowUp");

            migrationBuilder.RenameTable(
                name: "ClientFollowUp",
                newName: "ClientsFollowUps");

            migrationBuilder.RenameIndex(
                name: "IX_ClientFollowUp_CompanyId",
                table: "ClientsFollowUps",
                newName: "IX_ClientsFollowUps_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientFollowUp_ClientId",
                table: "ClientsFollowUps",
                newName: "IX_ClientsFollowUps_ClientId");

            migrationBuilder.AlterColumn<List<byte[]>>(
                name: "Images",
                table: "ClientsFollowUps",
                type: "bytea[]",
                nullable: true,
                oldClrType: typeof(List<byte[]>),
                oldType: "bytea[]");

            migrationBuilder.AddColumn<List<string>>(
                name: "ImagesContentType",
                table: "ClientsFollowUps",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientsFollowUps",
                table: "ClientsFollowUps",
                column: "ClientFollowUpId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientsFollowUps_Clients_ClientId",
                table: "ClientsFollowUps",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientsFollowUps_Companies_CompanyId",
                table: "ClientsFollowUps",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientsFollowUps_Clients_ClientId",
                table: "ClientsFollowUps");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientsFollowUps_Companies_CompanyId",
                table: "ClientsFollowUps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientsFollowUps",
                table: "ClientsFollowUps");

            migrationBuilder.DropColumn(
                name: "ImagesContentType",
                table: "ClientsFollowUps");

            migrationBuilder.RenameTable(
                name: "ClientsFollowUps",
                newName: "ClientFollowUp");

            migrationBuilder.RenameIndex(
                name: "IX_ClientsFollowUps_CompanyId",
                table: "ClientFollowUp",
                newName: "IX_ClientFollowUp_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientsFollowUps_ClientId",
                table: "ClientFollowUp",
                newName: "IX_ClientFollowUp_ClientId");

            migrationBuilder.AlterColumn<List<byte[]>>(
                name: "Images",
                table: "ClientFollowUp",
                type: "bytea[]",
                nullable: false,
                oldClrType: typeof(List<byte[]>),
                oldType: "bytea[]",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientFollowUp",
                table: "ClientFollowUp",
                column: "ClientFollowUpId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientFollowUp_Clients_ClientId",
                table: "ClientFollowUp",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientFollowUp_Companies_CompanyId",
                table: "ClientFollowUp",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
