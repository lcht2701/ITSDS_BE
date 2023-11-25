using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class Remove1IdInFeedbackTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_TicketSolutions_TicketSolutionId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_TicketSolutionId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "TicketSolutionId",
                table: "Feedbacks");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_SolutionId",
                table: "Feedbacks",
                column: "SolutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_TicketSolutions_SolutionId",
                table: "Feedbacks",
                column: "SolutionId",
                principalTable: "TicketSolutions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_TicketSolutions_SolutionId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_SolutionId",
                table: "Feedbacks");

            migrationBuilder.AddColumn<int>(
                name: "TicketSolutionId",
                table: "Feedbacks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_TicketSolutionId",
                table: "Feedbacks",
                column: "TicketSolutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_TicketSolutions_TicketSolutionId",
                table: "Feedbacks",
                column: "TicketSolutionId",
                principalTable: "TicketSolutions",
                principalColumn: "Id");
        }
    }
}
