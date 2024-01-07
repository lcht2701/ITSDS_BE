using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class RemoveDOBOfAccountAndRenameCompanyAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMembers_Departments_DepartmentId",
                table: "CompanyMembers");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "CompanyMembers",
                newName: "CompanyAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyMembers_DepartmentId",
                table: "CompanyMembers",
                newName: "IX_CompanyMembers_CompanyAddressId");

            migrationBuilder.CreateTable(
                name: "CompanyAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyAddresses_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAddresses_CompanyId",
                table: "CompanyAddresses",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAddresses_DeletedAt",
                table: "CompanyAddresses",
                column: "DeletedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMembers_CompanyAddresses_CompanyAddressId",
                table: "CompanyMembers",
                column: "CompanyAddressId",
                principalTable: "CompanyAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMembers_CompanyAddresses_CompanyAddressId",
                table: "CompanyMembers");

            migrationBuilder.DropTable(
                name: "CompanyAddresses");

            migrationBuilder.RenameColumn(
                name: "CompanyAddressId",
                table: "CompanyMembers",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyMembers_CompanyAddressId",
                table: "CompanyMembers",
                newName: "IX_CompanyMembers_DepartmentId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CompanyId",
                table: "Departments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DeletedAt",
                table: "Departments",
                column: "DeletedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMembers_Departments_DepartmentId",
                table: "CompanyMembers",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
