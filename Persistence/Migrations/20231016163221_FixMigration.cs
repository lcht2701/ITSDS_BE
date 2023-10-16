using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class FixMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Assignments_TicketId",
                table: "Assignments");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TicketId",
                table: "Assignments",
                column: "TicketId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Assignments_TicketId",
                table: "Assignments");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TicketId",
                table: "Assignments",
                column: "TicketId",
                unique: true,
                filter: "[TicketId] IS NOT NULL");
        }
    }
}
