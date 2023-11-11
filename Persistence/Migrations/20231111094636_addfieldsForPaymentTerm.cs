using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class addfieldsForPaymentTerm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Companies",
                newName: "IsActive");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PaymentTerms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "PaymentTerms",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "PaymentTerms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TermAmount",
                table: "PaymentTerms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TermEnd",
                table: "PaymentTerms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TermFinishTime",
                table: "PaymentTerms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TermStart",
                table: "PaymentTerms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFullyPaid",
                table: "Payments",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "PaymentTerms");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "PaymentTerms");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "PaymentTerms");

            migrationBuilder.DropColumn(
                name: "TermAmount",
                table: "PaymentTerms");

            migrationBuilder.DropColumn(
                name: "TermEnd",
                table: "PaymentTerms");

            migrationBuilder.DropColumn(
                name: "TermFinishTime",
                table: "PaymentTerms");

            migrationBuilder.DropColumn(
                name: "TermStart",
                table: "PaymentTerms");

            migrationBuilder.DropColumn(
                name: "IsFullyPaid",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Companies",
                newName: "isActive");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
