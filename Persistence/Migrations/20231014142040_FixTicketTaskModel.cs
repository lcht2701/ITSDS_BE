using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class FixTicketTaskModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalCost",
                table: "TicketTasks");

            migrationBuilder.DropColumn(
                name: "DateCompleted",
                table: "TicketTasks");

            migrationBuilder.RenameColumn(
                name: "TimeSpent",
                table: "TicketTasks",
                newName: "TaskStatus");

            migrationBuilder.AddColumn<int>(
                name: "CreateById",
                table: "TicketTasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketTasks_CreateById",
                table: "TicketTasks",
                column: "CreateById");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTasks_Users_CreateById",
                table: "TicketTasks",
                column: "CreateById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketTasks_Users_CreateById",
                table: "TicketTasks");

            migrationBuilder.DropIndex(
                name: "IX_TicketTasks_CreateById",
                table: "TicketTasks");

            migrationBuilder.DropColumn(
                name: "CreateById",
                table: "TicketTasks");

            migrationBuilder.RenameColumn(
                name: "TaskStatus",
                table: "TicketTasks",
                newName: "TimeSpent");

            migrationBuilder.AddColumn<double>(
                name: "AdditionalCost",
                table: "TicketTasks",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCompleted",
                table: "TicketTasks",
                type: "datetime2",
                nullable: true);
        }
    }
}
