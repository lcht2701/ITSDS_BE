using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class addManagerIdForTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Users_OwnerId",
                table: "Teams");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Teams",
                newName: "ManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_Teams_OwnerId",
                table: "Teams",
                newName: "IX_Teams_ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Users_ManagerId",
                table: "Teams",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Users_ManagerId",
                table: "Teams");

            migrationBuilder.RenameColumn(
                name: "ManagerId",
                table: "Teams",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Teams_ManagerId",
                table: "Teams",
                newName: "IX_Teams_OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Users_OwnerId",
                table: "Teams",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
