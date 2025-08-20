using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPropIsAccountVerifiedInEntityCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccountVerified",
                table: "Companies",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccountVerified",
                table: "Companies");
        }
    }
}
