using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class fixAttributeOfHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketStatus",
                table: "Histories");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Histories",
                newName: "Action");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Action",
                table: "Histories",
                newName: "Description");

            migrationBuilder.AddColumn<int>(
                name: "TicketStatus",
                table: "Histories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
