using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class addContractRenewals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "Contracts",
                newName: "ParentContractId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Contracts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRenewed",
                table: "Contracts",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Renewals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Value = table.Column<double>(type: "float", nullable: true),
                    RenewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RenewedById = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Renewals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Renewals_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Renewals_Users_RenewedById",
                        column: x => x.RenewedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_AccountantId",
                table: "Contracts",
                column: "AccountantId");

            migrationBuilder.CreateIndex(
                name: "IX_Renewals_ContractId",
                table: "Renewals",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Renewals_DeletedAt",
                table: "Renewals",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Renewals_RenewedById",
                table: "Renewals",
                column: "RenewedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Users_AccountantId",
                table: "Contracts",
                column: "AccountantId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Users_AccountantId",
                table: "Contracts");

            migrationBuilder.DropTable(
                name: "Renewals");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_AccountantId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "IsRenewed",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Contracts");

            migrationBuilder.RenameColumn(
                name: "ParentContractId",
                table: "Contracts",
                newName: "Duration");
        }
    }
}
