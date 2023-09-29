using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class addCompanyMemberToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyMembers_MemberId",
                table: "CompanyMembers");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMembers_MemberId",
                table: "CompanyMembers",
                column: "MemberId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyMembers_MemberId",
                table: "CompanyMembers");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMembers_MemberId",
                table: "CompanyMembers",
                column: "MemberId");
        }
    }
}
