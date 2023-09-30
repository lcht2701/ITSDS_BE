using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class changeCompanyMemberTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMember_Companies_CompanyId",
                table: "CompanyMember");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMember_Teams_TeamId",
                table: "TeamMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMember",
                table: "TeamMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyMember",
                table: "CompanyMember");

            migrationBuilder.RenameTable(
                name: "TeamMember",
                newName: "TeamMembers");

            migrationBuilder.RenameTable(
                name: "CompanyMember",
                newName: "CompanyMembers");

            migrationBuilder.RenameIndex(
                name: "IX_TeamMember_TeamId",
                table: "TeamMembers",
                newName: "IX_TeamMembers_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_TeamMember_DeletedAt",
                table: "TeamMembers",
                newName: "IX_TeamMembers_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyMember_DeletedAt",
                table: "CompanyMembers",
                newName: "IX_CompanyMembers_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyMember_CompanyId",
                table: "CompanyMembers",
                newName: "IX_CompanyMembers_CompanyId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyMembers",
                table: "CompanyMembers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMembers_Companies_CompanyId",
                table: "CompanyMembers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Teams_TeamId",
                table: "TeamMembers",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMembers_Companies_CompanyId",
                table: "CompanyMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Teams_TeamId",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyMembers",
                table: "CompanyMembers");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Teams");

            migrationBuilder.RenameTable(
                name: "TeamMembers",
                newName: "TeamMember");

            migrationBuilder.RenameTable(
                name: "CompanyMembers",
                newName: "CompanyMember");

            migrationBuilder.RenameIndex(
                name: "IX_TeamMembers_TeamId",
                table: "TeamMember",
                newName: "IX_TeamMember_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_TeamMembers_DeletedAt",
                table: "TeamMember",
                newName: "IX_TeamMember_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyMembers_DeletedAt",
                table: "CompanyMember",
                newName: "IX_CompanyMember_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyMembers_CompanyId",
                table: "CompanyMember",
                newName: "IX_CompanyMember_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMember",
                table: "TeamMember",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyMember",
                table: "CompanyMember",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMember_Companies_CompanyId",
                table: "CompanyMember",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMember_Teams_TeamId",
                table: "TeamMember",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
