using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orquestra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTableIntegrationWhatsapp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntegrationsWhatsapp",
                columns: table => new
                {
                    IntegrationWhatsappId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageReminderBeforeSchedule = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    MessageOnScheduleConfirmed = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    MessageOnScheduleCanceled = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    MessageBeforeScheduleAlert = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModificationBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationsWhatsapp", x => x.IntegrationWhatsappId);
                    table.ForeignKey(
                        name: "FK_IntegrationsWhatsapp_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationsWhatsapp_CompanyId",
                table: "IntegrationsWhatsapp",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntegrationsWhatsapp");
        }
    }
}
