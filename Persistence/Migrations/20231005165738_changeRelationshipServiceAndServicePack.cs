using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class changeRelationshipServiceAndServicePack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServicePacks_Services_ServiceId",
                table: "ServicePacks");

            migrationBuilder.DropIndex(
                name: "IX_ServicePacks_ServiceId",
                table: "ServicePacks");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "ServicePacks");

            migrationBuilder.AddColumn<int>(
                name: "ServicePackId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServicePackId",
                table: "Services",
                column: "ServicePackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServicePacks_ServicePackId",
                table: "Services",
                column: "ServicePackId",
                principalTable: "ServicePacks",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServicePacks_ServicePackId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_ServicePackId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ServicePackId",
                table: "Services");

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "ServicePacks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServicePacks_ServiceId",
                table: "ServicePacks",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicePacks_Services_ServiceId",
                table: "ServicePacks",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id");
        }
    }
}
