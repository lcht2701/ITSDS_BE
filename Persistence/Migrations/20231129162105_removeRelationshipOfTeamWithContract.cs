using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class removeRelationshipOfTeamWithContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Teams_TeamId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_TeamId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Contracts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Contracts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_TeamId",
                table: "Contracts",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Teams_TeamId",
                table: "Contracts",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }
    }
}
