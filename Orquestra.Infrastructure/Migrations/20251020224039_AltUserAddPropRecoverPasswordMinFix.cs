using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AltUserAddPropRecoverPasswordMinFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecoverPasswordAswer",
                table: "Users",
                newName: "RecoverPasswordAnswer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecoverPasswordAnswer",
                table: "Users",
                newName: "RecoverPasswordAswer");
        }
    }
}
