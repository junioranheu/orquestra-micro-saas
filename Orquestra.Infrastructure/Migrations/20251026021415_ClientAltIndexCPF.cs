using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ClientAltIndexCPF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clients_CompanyId_CPF",
                table: "Clients");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CompanyId_CPF",
                table: "Clients",
                columns: new[] { "CompanyId", "CPF" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clients_CompanyId_CPF",
                table: "Clients");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CompanyId_CPF",
                table: "Clients",
                columns: new[] { "CompanyId", "CPF" },
                unique: true);
        }
    }
}
