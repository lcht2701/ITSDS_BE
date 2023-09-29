using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class fixCompanyUserCompanyMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMembers_MemberId",
                table: "CompanyMembers",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMembers_Users_MemberId",
                table: "CompanyMembers",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMembers_Users_MemberId",
                table: "CompanyMembers");

            migrationBuilder.DropIndex(
                name: "IX_CompanyMembers_MemberId",
                table: "CompanyMembers");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Users");
        }
    }
}
