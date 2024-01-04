using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class UpdateCompanyCompanyMemberTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Users_CustomerAdminId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMembers_Companies_CompanyId",
                table: "CompanyMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMembers_Users_MemberId",
                table: "CompanyMembers");

            migrationBuilder.DropIndex(
                name: "IX_Companies_CustomerAdminId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CustomerAdminId",
                table: "Companies");

            migrationBuilder.AlterColumn<int>(
                name: "MemberId",
                table: "CompanyMembers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "CompanyMembers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompanyAdmin",
                table: "CompanyMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMembers_Companies_CompanyId",
                table: "CompanyMembers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_CompanyMembers_Companies_CompanyId",
                table: "CompanyMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMembers_Users_MemberId",
                table: "CompanyMembers");

            migrationBuilder.DropColumn(
                name: "IsCompanyAdmin",
                table: "CompanyMembers");

            migrationBuilder.AlterColumn<int>(
                name: "MemberId",
                table: "CompanyMembers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "CompanyMembers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CustomerAdminId",
                table: "Companies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CustomerAdminId",
                table: "Companies",
                column: "CustomerAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Users_CustomerAdminId",
                table: "Companies",
                column: "CustomerAdminId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMembers_Companies_CompanyId",
                table: "CompanyMembers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMembers_Users_MemberId",
                table: "CompanyMembers",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
