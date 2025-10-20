using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AltVerificationAddPropExpirationDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Reference",
                table: "Verifications",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "Verifications",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Verifications");

            migrationBuilder.AlterColumn<string>(
                name: "Reference",
                table: "Verifications",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);
        }
    }
}
