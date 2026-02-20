using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Aea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrder_Clients_ClientId",
                table: "ServiceOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrder_Companies_CompanyId",
                table: "ServiceOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrder_Quotes_QuoteId",
                table: "ServiceOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceOrder",
                table: "ServiceOrder");

            migrationBuilder.RenameTable(
                name: "ServiceOrder",
                newName: "ServiceOrders");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceOrder_QuoteId",
                table: "ServiceOrders",
                newName: "IX_ServiceOrders_QuoteId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceOrder_CompanyId",
                table: "ServiceOrders",
                newName: "IX_ServiceOrders_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceOrder_ClientId",
                table: "ServiceOrders",
                newName: "IX_ServiceOrders_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceOrders",
                table: "ServiceOrders",
                column: "ServiceOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_Clients_ClientId",
                table: "ServiceOrders",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_Companies_CompanyId",
                table: "ServiceOrders",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_Quotes_QuoteId",
                table: "ServiceOrders",
                column: "QuoteId",
                principalTable: "Quotes",
                principalColumn: "QuoteId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Clients_ClientId",
                table: "ServiceOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Companies_CompanyId",
                table: "ServiceOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_Quotes_QuoteId",
                table: "ServiceOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceOrders",
                table: "ServiceOrders");

            migrationBuilder.RenameTable(
                name: "ServiceOrders",
                newName: "ServiceOrder");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceOrders_QuoteId",
                table: "ServiceOrder",
                newName: "IX_ServiceOrder_QuoteId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceOrders_CompanyId",
                table: "ServiceOrder",
                newName: "IX_ServiceOrder_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceOrders_ClientId",
                table: "ServiceOrder",
                newName: "IX_ServiceOrder_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceOrder",
                table: "ServiceOrder",
                column: "ServiceOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrder_Clients_ClientId",
                table: "ServiceOrder",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrder_Companies_CompanyId",
                table: "ServiceOrder",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrder_Quotes_QuoteId",
                table: "ServiceOrder",
                column: "QuoteId",
                principalTable: "Quotes",
                principalColumn: "QuoteId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
