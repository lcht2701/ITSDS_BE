using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class add1TeamManage1Category : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_AssignedTechnicalId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_AssignedTechnicalId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "AssignedTechnicalId",
                table: "Categories");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Teams",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CategoryId",
                table: "Teams",
                column: "CategoryId",
                unique: true,
                filter: "[CategoryId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Categories_CategoryId",
                table: "Teams",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Categories_CategoryId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_CategoryId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Teams");

            migrationBuilder.AddColumn<int>(
                name: "AssignedTechnicalId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_AssignedTechnicalId",
                table: "Categories",
                column: "AssignedTechnicalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_AssignedTechnicalId",
                table: "Categories",
                column: "AssignedTechnicalId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
