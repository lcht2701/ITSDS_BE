using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AddTeamMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Teams_TeamId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_CompanyMembers_CompanyMemberId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMembers_Users_MemberId",
                table: "CompanyMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Assignments_AssignmentId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_AssignmentId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Companies_CompanyMemberId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_CustomerAdminId",
                table: "Companies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyMembers",
                table: "CompanyMembers");

            migrationBuilder.DropIndex(
                name: "IX_CompanyMembers_MemberId",
                table: "CompanyMembers");

            migrationBuilder.DropColumn(
                name: "AssignmentId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CompanyMemberId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Assignments");

            migrationBuilder.RenameTable(
                name: "CompanyMembers",
                newName: "CompanyMember");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Tickets",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "Tickets",
                newName: "TechnicianNote");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Tickets",
                newName: "EstimatedFinishTime");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "Assignments",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Assignments",
                newName: "TicketId");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "Assignments",
                newName: "TechnicianId");

            migrationBuilder.RenameIndex(
                name: "IX_Assignments_TeamId",
                table: "Assignments",
                newName: "IX_Assignments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyMembers_DeletedAt",
                table: "CompanyMember",
                newName: "IX_CompanyMember_DeletedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualFinishTime",
                table: "Tickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AttachmentUrl",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequesterNote",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamMemberId",
                table: "Teams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "CompanyMember",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyMember",
                table: "CompanyMember",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TeamMember",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    Expertises = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeamId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMember", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TeamId",
                table: "Tickets",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_TeamMemberId",
                table: "Teams",
                column: "TeamMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CustomerAdminId",
                table: "Companies",
                column: "CustomerAdminId",
                unique: true,
                filter: "[CustomerAdminId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TicketId",
                table: "Assignments",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMember_CompanyId",
                table: "CompanyMember",
                column: "CompanyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMember_MemberId",
                table: "CompanyMember",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_DeletedAt",
                table: "TeamMember",
                column: "DeletedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Tickets_TicketId",
                table: "Assignments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Users_UserId",
                table: "Assignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMember_Companies_CompanyId",
                table: "CompanyMember",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMember_Users_MemberId",
                table: "CompanyMember",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_TeamMember_TeamMemberId",
                table: "Teams",
                column: "TeamMemberId",
                principalTable: "TeamMember",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Teams_TeamId",
                table: "Tickets",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Tickets_TicketId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Users_UserId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMember_Companies_CompanyId",
                table: "CompanyMember");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMember_Users_MemberId",
                table: "CompanyMember");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_TeamMember_TeamMemberId",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Teams_TeamId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "TeamMember");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TeamId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Teams_TeamMemberId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Companies_CustomerAdminId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_TicketId",
                table: "Assignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyMember",
                table: "CompanyMember");

            migrationBuilder.DropIndex(
                name: "IX_CompanyMember_CompanyId",
                table: "CompanyMember");

            migrationBuilder.DropIndex(
                name: "IX_CompanyMember_MemberId",
                table: "CompanyMember");

            migrationBuilder.DropColumn(
                name: "ActualFinishTime",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "AttachmentUrl",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "RequesterNote",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TeamMemberId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "CompanyMember");

            migrationBuilder.RenameTable(
                name: "CompanyMember",
                newName: "CompanyMembers");

            migrationBuilder.RenameColumn(
                name: "TechnicianNote",
                table: "Tickets",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "Tickets",
                newName: "ServiceId");

            migrationBuilder.RenameColumn(
                name: "EstimatedFinishTime",
                table: "Tickets",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Assignments",
                newName: "TeamId");

            migrationBuilder.RenameColumn(
                name: "TicketId",
                table: "Assignments",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "TechnicianId",
                table: "Assignments",
                newName: "Priority");

            migrationBuilder.RenameIndex(
                name: "IX_Assignments_UserId",
                table: "Assignments",
                newName: "IX_Assignments_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyMember_DeletedAt",
                table: "CompanyMembers",
                newName: "IX_CompanyMembers_DeletedAt");

            migrationBuilder.AddColumn<int>(
                name: "AssignmentId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyMemberId",
                table: "Companies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyMembers",
                table: "CompanyMembers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AssignmentId",
                table: "Tickets",
                column: "AssignmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CompanyMemberId",
                table: "Companies",
                column: "CompanyMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CustomerAdminId",
                table: "Companies",
                column: "CustomerAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMembers_MemberId",
                table: "CompanyMembers",
                column: "MemberId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Teams_TeamId",
                table: "Assignments",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_CompanyMembers_CompanyMemberId",
                table: "Companies",
                column: "CompanyMemberId",
                principalTable: "CompanyMembers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMembers_Users_MemberId",
                table: "CompanyMembers",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Assignments_AssignmentId",
                table: "Tickets",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
