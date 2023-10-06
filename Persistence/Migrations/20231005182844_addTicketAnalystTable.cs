using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class addTicketAnalystTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketTasks_Users_TechnicianId",
                table: "TicketTasks");

            migrationBuilder.DropIndex(
                name: "IX_TicketTasks_TechnicianId",
                table: "TicketTasks");

            migrationBuilder.CreateTable(
                name: "TicketAnalysts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: true),
                    Impact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RootCause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Symptoms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Attachments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Solution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketAnalysts");

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
    }
}
