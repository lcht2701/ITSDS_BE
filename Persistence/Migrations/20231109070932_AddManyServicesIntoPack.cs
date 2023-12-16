using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AddManyServicesIntoPack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServicePacks_ServicePackId",
                table: "Services");

            migrationBuilder.DropTable(
                name: "ContractDetails");

            migrationBuilder.DropIndex(
                name: "IX_Services_ServicePackId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ServicePackId",
                table: "Services");

            migrationBuilder.CreateTable(
                name: "ContractServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: true),
                    ServiceId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractServices_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ServiceServicePacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicePackId = table.Column<int>(type: "int", nullable: true),
                    ServiceId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceServicePacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceServicePacks_ServicePacks_ServicePackId",
                        column: x => x.ServicePackId,
                        principalTable: "ServicePacks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceServicePacks_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractServices_ContractId",
                table: "ContractServices",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractServices_DeletedAt",
                table: "ContractServices",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContractServices_ServiceId",
                table: "ContractServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceServicePacks_DeletedAt",
                table: "ServiceServicePacks",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceServicePacks_ServiceId",
                table: "ServiceServicePacks",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceServicePacks_ServicePackId",
                table: "ServiceServicePacks",
                column: "ServicePackId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractServices");

            migrationBuilder.DropTable(
                name: "ServiceServicePacks");

            migrationBuilder.AddColumn<int>(
                name: "ServicePackId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContractDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdditionalServiceId = table.Column<int>(type: "int", nullable: true),
                    ContractId = table.Column<int>(type: "int", nullable: true),
                    ServicePackId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractDetails_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractDetails_ServicePacks_ServicePackId",
                        column: x => x.ServicePackId,
                        principalTable: "ServicePacks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractDetails_Services_AdditionalServiceId",
                        column: x => x.AdditionalServiceId,
                        principalTable: "Services",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServicePackId",
                table: "Services",
                column: "ServicePackId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDetails_AdditionalServiceId",
                table: "ContractDetails",
                column: "AdditionalServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDetails_ContractId",
                table: "ContractDetails",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDetails_DeletedAt",
                table: "ContractDetails",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDetails_ServicePackId",
                table: "ContractDetails",
                column: "ServicePackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServicePacks_ServicePackId",
                table: "Services",
                column: "ServicePackId",
                principalTable: "ServicePacks",
                principalColumn: "Id");
        }
    }
}
