using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class removeRelationshipsOfApprovalWithUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketApprovals_Users_ApprovalCreaterId",
                table: "TicketApprovals");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketApprovals_Users_ApproverId",
                table: "TicketApprovals");

            migrationBuilder.DropIndex(
                name: "IX_TicketApprovals_ApprovalCreaterId",
                table: "TicketApprovals");

            migrationBuilder.DropIndex(
                name: "IX_TicketApprovals_ApproverId",
                table: "TicketApprovals");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TicketApprovals_ApprovalCreaterId",
                table: "TicketApprovals",
                column: "ApprovalCreaterId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketApprovals_ApproverId",
                table: "TicketApprovals",
                column: "ApproverId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketApprovals_Users_ApprovalCreaterId",
                table: "TicketApprovals",
                column: "ApprovalCreaterId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketApprovals_Users_ApproverId",
                table: "TicketApprovals",
                column: "ApproverId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
