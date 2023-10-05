using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class fixTableRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMembers_Companies_CompanyId",
                table: "CompanyMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMembers_Users_MemberId",
                table: "CompanyMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractDetails_Contracts_ContractId",
                table: "ContractDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Companies_CompanyId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Teams_TeamId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Contracts_ContractId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_ContractDetails_ContractDetailId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServicePacks_ServicePackId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketApprovals_Tickets_TicketId",
                table: "TicketApprovals");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_ServicePacks_ServicePackId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketTasks_Tickets_TicketId",
                table: "TicketTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketTasks_Users_TechnicianId",
                table: "TicketTasks");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_ServicePackId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Services_ContractDetailId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_ServicePackId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ServicePackId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ContractDetailId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ServicePackId",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Users",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "isDone",
                table: "TicketTasks",
                newName: "IsDone");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDone",
                table: "TicketTasks",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TicketTasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "TicketTasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TechnicianId",
                table: "TicketTasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TaskStatus",
                table: "TicketTasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TicketApprovals",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "TicketApprovals",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Amount",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ServicePacks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Price",
                table: "ServicePacks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "ServicePacks",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentStart",
                table: "Payments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentEnd",
                table: "Payments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ContractId",
                table: "Payments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "Value",
                table: "Contracts",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "Contracts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Contracts",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "Contracts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Contracts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ContractId",
                table: "ContractDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AdditionalServiceId",
                table: "ContractDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MemberPosition",
                table: "CompanyMembers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "MemberId",
                table: "CompanyMembers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "CompanyMembers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Website",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TaxCode",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePacks_ServiceId",
                table: "ServicePacks",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDetails_AdditionalServiceId",
                table: "ContractDetails",
                column: "AdditionalServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMembers_Companies_CompanyId",
                table: "CompanyMembers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMembers_Users_MemberId",
                table: "CompanyMembers",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractDetails_Contracts_ContractId",
                table: "ContractDetails",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractDetails_Services_AdditionalServiceId",
                table: "ContractDetails",
                column: "AdditionalServiceId",
                principalTable: "Services",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Companies_CompanyId",
                table: "Contracts",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Teams_TeamId",
                table: "Contracts",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Contracts_ContractId",
                table: "Payments",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicePacks_Services_ServiceId",
                table: "ServicePacks",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketApprovals_Tickets_TicketId",
                table: "TicketApprovals",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTasks_Tickets_TicketId",
                table: "TicketTasks",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTasks_Users_TechnicianId",
                table: "TicketTasks",
                column: "TechnicianId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMembers_Companies_CompanyId",
                table: "CompanyMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMembers_Users_MemberId",
                table: "CompanyMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractDetails_Contracts_ContractId",
                table: "ContractDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractDetails_Services_AdditionalServiceId",
                table: "ContractDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Companies_CompanyId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Teams_TeamId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Contracts_ContractId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicePacks_Services_ServiceId",
                table: "ServicePacks");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketApprovals_Tickets_TicketId",
                table: "TicketApprovals");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketTasks_Tickets_TicketId",
                table: "TicketTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketTasks_Users_TechnicianId",
                table: "TicketTasks");

            migrationBuilder.DropIndex(
                name: "IX_ServicePacks_ServiceId",
                table: "ServicePacks");

            migrationBuilder.DropIndex(
                name: "IX_ContractDetails_AdditionalServiceId",
                table: "ContractDetails");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ServicePacks");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "ServicePacks");

            migrationBuilder.DropColumn(
                name: "AdditionalServiceId",
                table: "ContractDetails");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Users",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "IsDone",
                table: "TicketTasks",
                newName: "isDone");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TicketTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "TicketTasks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TechnicianId",
                table: "TicketTasks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TaskStatus",
                table: "TicketTasks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "isDone",
                table: "TicketTasks",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServicePackId",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TicketApprovals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "TicketApprovals",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Services",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Services",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Amount",
                table: "Services",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContractDetailId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServicePackId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ServicePacks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentStart",
                table: "Payments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentEnd",
                table: "Payments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ContractId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Value",
                table: "Contracts",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Contracts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ContractId",
                table: "ContractDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MemberPosition",
                table: "CompanyMembers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MemberId",
                table: "CompanyMembers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "CompanyMembers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Website",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TaxCode",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ServicePackId",
                table: "Tickets",
                column: "ServicePackId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ContractDetailId",
                table: "Services",
                column: "ContractDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServicePackId",
                table: "Services",
                column: "ServicePackId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMembers_Companies_CompanyId",
                table: "CompanyMembers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMembers_Users_MemberId",
                table: "CompanyMembers",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractDetails_Contracts_ContractId",
                table: "ContractDetails",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Companies_CompanyId",
                table: "Contracts",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Teams_TeamId",
                table: "Contracts",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Contracts_ContractId",
                table: "Payments",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ContractDetails_ContractDetailId",
                table: "Services",
                column: "ContractDetailId",
                principalTable: "ContractDetails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServicePacks_ServicePackId",
                table: "Services",
                column: "ServicePackId",
                principalTable: "ServicePacks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketApprovals_Tickets_TicketId",
                table: "TicketApprovals",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_ServicePacks_ServicePackId",
                table: "Tickets",
                column: "ServicePackId",
                principalTable: "ServicePacks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTasks_Tickets_TicketId",
                table: "TicketTasks",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTasks_Users_TechnicianId",
                table: "TicketTasks",
                column: "TechnicianId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
