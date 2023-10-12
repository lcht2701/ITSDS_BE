using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class createRelationshipBetweenSokutionAndUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TicketSolutions_OwnerId",
                table: "TicketSolutions",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketSolutions_Users_OwnerId",
                table: "TicketSolutions",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketSolutions_Users_OwnerId",
                table: "TicketSolutions");

            migrationBuilder.DropIndex(
                name: "IX_TicketSolutions_OwnerId",
                table: "TicketSolutions");
        }
    }
}
