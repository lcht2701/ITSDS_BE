using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class removeFrequencyInServicesContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "ServiceContracts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "ServiceContracts",
                type: "int",
                nullable: true);
        }
    }
}
