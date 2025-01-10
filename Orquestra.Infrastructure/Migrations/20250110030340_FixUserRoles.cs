using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosRoles_Users_UserId",
                table: "UsuariosRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuariosRoles",
                table: "UsuariosRoles");

            migrationBuilder.RenameTable(
                name: "UsuariosRoles",
                newName: "UserRoles");

            migrationBuilder.RenameIndex(
                name: "IX_UsuariosRoles_UserId",
                table: "UserRoles",
                newName: "IX_UserRoles_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                column: "UserRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "UsuariosRoles");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_UserId",
                table: "UsuariosRoles",
                newName: "IX_UsuariosRoles_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuariosRoles",
                table: "UsuariosRoles",
                column: "UserRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosRoles_Users_UserId",
                table: "UsuariosRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
