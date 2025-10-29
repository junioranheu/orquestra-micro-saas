using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AltTableIntegrationWhatsApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IntegrationsWhatsapp_Companies_CompanyId",
                table: "IntegrationsWhatsapp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IntegrationsWhatsapp",
                table: "IntegrationsWhatsapp");

            migrationBuilder.RenameTable(
                name: "IntegrationsWhatsapp",
                newName: "IntegrationsWhatsApp");

            migrationBuilder.RenameColumn(
                name: "IntegrationWhatsappId",
                table: "IntegrationsWhatsApp",
                newName: "IntegrationWhatsAppId");

            migrationBuilder.RenameIndex(
                name: "IX_IntegrationsWhatsapp_CompanyId",
                table: "IntegrationsWhatsApp",
                newName: "IX_IntegrationsWhatsApp_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IntegrationsWhatsApp",
                table: "IntegrationsWhatsApp",
                column: "IntegrationWhatsAppId");

            migrationBuilder.AddForeignKey(
                name: "FK_IntegrationsWhatsApp_Companies_CompanyId",
                table: "IntegrationsWhatsApp",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IntegrationsWhatsApp_Companies_CompanyId",
                table: "IntegrationsWhatsApp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IntegrationsWhatsApp",
                table: "IntegrationsWhatsApp");

            migrationBuilder.RenameTable(
                name: "IntegrationsWhatsApp",
                newName: "IntegrationsWhatsapp");

            migrationBuilder.RenameColumn(
                name: "IntegrationWhatsAppId",
                table: "IntegrationsWhatsapp",
                newName: "IntegrationWhatsappId");

            migrationBuilder.RenameIndex(
                name: "IX_IntegrationsWhatsApp_CompanyId",
                table: "IntegrationsWhatsapp",
                newName: "IX_IntegrationsWhatsapp_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IntegrationsWhatsapp",
                table: "IntegrationsWhatsapp",
                column: "IntegrationWhatsappId");

            migrationBuilder.AddForeignKey(
                name: "FK_IntegrationsWhatsapp_Companies_CompanyId",
                table: "IntegrationsWhatsapp",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
