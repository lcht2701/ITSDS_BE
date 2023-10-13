using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AddAssignmentIdToTicket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Tickets_TicketId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_TicketId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "Assignments");

            migrationBuilder.AddColumn<int>(
                name: "AssignmentId",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AssignmentId",
                table: "Tickets",
                column: "AssignmentId",
                unique: true,
                filter: "[AssignmentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Assignments_AssignmentId",
                table: "Tickets",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Assignments_AssignmentId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_AssignmentId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "AssignmentId",
                table: "Tickets");

            migrationBuilder.AddColumn<int>(
                name: "TicketId",
                table: "Assignments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TicketId",
                table: "Assignments",
                column: "TicketId",
                unique: true,
                filter: "[TicketId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Tickets_TicketId",
                table: "Assignments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id");
        }
    }
}
