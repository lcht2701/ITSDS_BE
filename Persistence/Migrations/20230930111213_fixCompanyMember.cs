using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class fixCompanyMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMember_Users_MemberId",
                table: "CompanyMember");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Users_UserId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_TeamMember_TeamMemberId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_TeamMemberId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_CompanyMember_MemberId",
                table: "CompanyMember");

            migrationBuilder.DropColumn(
                name: "TeamMemberId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Feedbacks");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "TeamMember",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_TeamId",
                table: "TeamMember",
                column: "TeamId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMember_Teams_TeamId",
                table: "TeamMember",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMember_Teams_TeamId",
                table: "TeamMember");

            migrationBuilder.DropIndex(
                name: "IX_TeamMember_TeamId",
                table: "TeamMember");

            migrationBuilder.AddColumn<int>(
                name: "TeamMemberId",
                table: "Teams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "TeamId",
                table: "TeamMember",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Feedbacks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_TeamMemberId",
                table: "Teams",
                column: "TeamMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMember_MemberId",
                table: "CompanyMember",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMember_Users_MemberId",
                table: "CompanyMember",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Users_UserId",
                table: "Feedbacks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_TeamMember_TeamMemberId",
                table: "Teams",
                column: "TeamMemberId",
                principalTable: "TeamMember",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
