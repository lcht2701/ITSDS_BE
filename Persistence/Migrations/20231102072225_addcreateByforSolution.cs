using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class addcreateByforSolution : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "TicketSolutions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketSolutions_CreatedById",
                table: "TicketSolutions",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketSolutions_Users_CreatedById",
                table: "TicketSolutions",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketSolutions_Users_CreatedById",
                table: "TicketSolutions");

            migrationBuilder.DropIndex(
                name: "IX_TicketSolutions_CreatedById",
                table: "TicketSolutions");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "TicketSolutions");
        }
    }
}
