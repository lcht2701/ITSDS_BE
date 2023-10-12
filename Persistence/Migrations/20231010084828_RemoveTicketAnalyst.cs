using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class RemoveTicketAnalyst : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketAnalysts");

            migrationBuilder.AddColumn<string>(
                name: "ImpactDetail",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketTasks_TechnicianId",
                table: "TicketTasks",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTasks_Users_TechnicianId",
                table: "TicketTasks",
                column: "TechnicianId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketTasks_Users_TechnicianId",
                table: "TicketTasks");

            migrationBuilder.DropIndex(
                name: "IX_TicketTasks_TechnicianId",
                table: "TicketTasks");

            migrationBuilder.DropColumn(
                name: "ImpactDetail",
                table: "Tickets");

            migrationBuilder.CreateTable(
                name: "TicketAnalysts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: true),
                    Attachments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Impact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RootCause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Symptoms = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketAnalysts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketAnalysts_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketAnalysts_DeletedAt",
                table: "TicketAnalysts",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAnalysts_TicketId",
                table: "TicketAnalysts",
                column: "TicketId");
        }
    }
}
