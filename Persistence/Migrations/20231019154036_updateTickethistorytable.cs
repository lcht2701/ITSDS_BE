using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class updateTickethistorytable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Histories");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_TicketId",
                table: "Assignments");

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

            migrationBuilder.CreateTable(
                name: "TicketHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    TicketId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketHistories_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TicketHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketTasks_CreateById",
                table: "TicketTasks",
                column: "CreateById");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TicketId",
                table: "Assignments",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistories_TicketId",
                table: "TicketHistories",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistories_UserId",
                table: "TicketHistories",
                column: "UserId");

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

            migrationBuilder.DropTable(
                name: "TicketHistories");

            migrationBuilder.DropIndex(
                name: "IX_TicketTasks_CreateById",
                table: "TicketTasks");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_TicketId",
                table: "Assignments");

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

            migrationBuilder.CreateTable(
                name: "Histories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Histories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Histories_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TicketId",
                table: "Assignments",
                column: "TicketId",
                unique: true,
                filter: "[TicketId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Histories_DeletedAt",
                table: "Histories",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Histories_TicketId",
                table: "Histories",
                column: "TicketId");
        }
    }
}
