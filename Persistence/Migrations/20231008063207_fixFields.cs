using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class fixFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Assignments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TeamId",
                table: "Assignments",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Teams_TeamId",
                table: "Assignments",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Teams_TeamId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_TeamId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Assignments");
        }
    }
}
