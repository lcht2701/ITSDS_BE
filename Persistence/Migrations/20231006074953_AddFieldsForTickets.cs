using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AddFieldsForTickets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Companies_CompanyId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "TicketApprovals");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_CompanyId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "IsDone",
                table: "TicketTasks");

            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "Tickets",
                newName: "ScheduledStartTime");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "Tickets",
                newName: "Urgency");

            migrationBuilder.RenameColumn(
                name: "ClosedDate",
                table: "Tickets",
                newName: "ScheduledEndTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualEndTime",
                table: "TicketTasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualStartTime",
                table: "TicketTasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AdditionalCost",
                table: "TicketTasks",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentUrl",
                table: "TicketTasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "TicketTasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Progress",
                table: "TicketTasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "TicketTasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedTime",
                table: "Tickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueTime",
                table: "Tickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Impact",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModeId",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<int>(type: "int", nullable: true),
                    AssignedTechnicalId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Users_AssignedTechnicalId",
                        column: x => x.AssignedTechnicalId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Histories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    TicketStatus = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Modes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketTasks_TeamId",
                table: "TicketTasks",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CategoryId",
                table: "Tickets",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ModeId",
                table: "Tickets",
                column: "ModeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TeamId",
                table: "Tickets",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_AssignedTechnicalId",
                table: "Categories",
                column: "AssignedTechnicalId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_DeletedAt",
                table: "Categories",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Histories_DeletedAt",
                table: "Histories",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Histories_TicketId",
                table: "Histories",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Modes_DeletedAt",
                table: "Modes",
                column: "DeletedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Categories_CategoryId",
                table: "Tickets",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Modes_ModeId",
                table: "Tickets",
                column: "ModeId",
                principalTable: "Modes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Teams_TeamId",
                table: "Tickets",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTasks_Teams_TeamId",
                table: "TicketTasks",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Categories_CategoryId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Modes_ModeId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Teams_TeamId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketTasks_Teams_TeamId",
                table: "TicketTasks");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Histories");

            migrationBuilder.DropTable(
                name: "Modes");

            migrationBuilder.DropIndex(
                name: "IX_TicketTasks_TeamId",
                table: "TicketTasks");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_CategoryId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_ModeId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TeamId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ActualEndTime",
                table: "TicketTasks");

            migrationBuilder.DropColumn(
                name: "ActualStartTime",
                table: "TicketTasks");

            migrationBuilder.DropColumn(
                name: "AdditionalCost",
                table: "TicketTasks");

            migrationBuilder.DropColumn(
                name: "AttachmentUrl",
                table: "TicketTasks");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "TicketTasks");

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "TicketTasks");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "TicketTasks");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CompletedTime",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DueTime",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Impact",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ModeId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "Urgency",
                table: "Tickets",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "ScheduledStartTime",
                table: "Tickets",
                newName: "DueDate");

            migrationBuilder.RenameColumn(
                name: "ScheduledEndTime",
                table: "Tickets",
                newName: "ClosedDate");

            migrationBuilder.AddColumn<bool>(
                name: "IsDone",
                table: "TicketTasks",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TicketApprovals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: true),
                    ApprovalCreaterId = table.Column<int>(type: "int", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApproverId = table.Column<int>(type: "int", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Stage = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketApprovals_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CompanyId",
                table: "Tickets",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketApprovals_DeletedAt",
                table: "TicketApprovals",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TicketApprovals_TicketId",
                table: "TicketApprovals",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Companies_CompanyId",
                table: "Tickets",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }
    }
}
