using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class removeServicePacks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceServicePacks");

            migrationBuilder.DropTable(
                name: "ServicePacks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServicePacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Price = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceServicePacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<int>(type: "int", nullable: true),
                    ServicePackId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "IX_ServicePacks_DeletedAt",
                table: "ServicePacks",
                column: "DeletedAt");

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
    }
}
