using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class SeperateTicketLocationInto4Fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Tickets",
                newName: "Ward");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "Ward",
                table: "Tickets",
                newName: "Location");
        }
    }
}
