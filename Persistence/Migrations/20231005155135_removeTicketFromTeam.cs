using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class removeTicketFromTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Teams_TeamId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TeamId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Tickets");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TeamId",
                table: "Tickets",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Teams_TeamId",
                table: "Tickets",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }
    }
}
