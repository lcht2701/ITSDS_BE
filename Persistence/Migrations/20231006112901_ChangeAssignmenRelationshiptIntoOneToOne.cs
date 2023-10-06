using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class ChangeAssignmenRelationshiptIntoOneToOne : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Tickets_TicketId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_TicketId",
                table: "Assignments");

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "Assignments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Tickets_TicketId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_TicketId",
                table: "Assignments");

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "Assignments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TicketId",
                table: "Assignments",
                column: "TicketId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Tickets_TicketId",
                table: "Assignments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
