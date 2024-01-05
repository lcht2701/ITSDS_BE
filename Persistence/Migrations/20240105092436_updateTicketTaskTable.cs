using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class updateTicketTaskTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketTasks_Teams_TeamId",
                table: "TicketTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketTasks_Users_TechnicianId",
                table: "TicketTasks");

            migrationBuilder.DropIndex(
                name: "IX_TicketTasks_TeamId",
                table: "TicketTasks");

            migrationBuilder.DropIndex(
                name: "IX_TicketTasks_TechnicianId",
                table: "TicketTasks");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "TicketTasks");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "TicketTasks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "TicketTasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TechnicianId",
                table: "TicketTasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketTasks_TeamId",
                table: "TicketTasks",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTasks_TechnicianId",
                table: "TicketTasks",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTasks_Teams_TeamId",
                table: "TicketTasks",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTasks_Users_TechnicianId",
                table: "TicketTasks",
                column: "TechnicianId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
