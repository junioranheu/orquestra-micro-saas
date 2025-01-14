using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCompany2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangePasswordCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ChangePasswordCodeValidity",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Adress",
                table: "Companies",
                newName: "Address");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Companies",
                newName: "Adress");

            migrationBuilder.AddColumn<string>(
                name: "ChangePasswordCode",
                table: "Users",
                type: "varchar(25)",
                maxLength: 25,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ChangePasswordCodeValidity",
                table: "Users",
                type: "datetime(6)",
                nullable: true);
        }
    }
}
