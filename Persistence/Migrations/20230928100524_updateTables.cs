using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class updateTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_CompanyMember_CompanyMemberId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Companies_CompanyId",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Teams_TeamId",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractDetail_Contract_ContractId",
                table: "ContractDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractDetail_ServicePack_ServicePackId",
                table: "ContractDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Contract_ContractId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTerm_Payment_PaymentId",
                table: "PaymentTerm");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_ContractDetail_ContractDetailId",
                table: "Service");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_ServicePack_ServicePackId",
                table: "Service");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServicePack",
                table: "ServicePack");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Service",
                table: "Service");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentTerm",
                table: "PaymentTerm");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContractDetail",
                table: "ContractDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contract",
                table: "Contract");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyMember",
                table: "CompanyMember");

            migrationBuilder.RenameTable(
                name: "ServicePack",
                newName: "ServicePacks");

            migrationBuilder.RenameTable(
                name: "Service",
                newName: "Services");

            migrationBuilder.RenameTable(
                name: "PaymentTerm",
                newName: "PaymentTerms");

            migrationBuilder.RenameTable(
                name: "Payment",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "ContractDetail",
                newName: "ContractDetails");

            migrationBuilder.RenameTable(
                name: "Contract",
                newName: "Contracts");

            migrationBuilder.RenameTable(
                name: "CompanyMember",
                newName: "CompanyMembers");

            migrationBuilder.RenameIndex(
                name: "IX_ServicePack_DeletedAt",
                table: "ServicePacks",
                newName: "IX_ServicePacks_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Service_ServicePackId",
                table: "Services",
                newName: "IX_Services_ServicePackId");

            migrationBuilder.RenameIndex(
                name: "IX_Service_DeletedAt",
                table: "Services",
                newName: "IX_Services_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Service_ContractDetailId",
                table: "Services",
                newName: "IX_Services_ContractDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentTerm_PaymentId",
                table: "PaymentTerms",
                newName: "IX_PaymentTerms_PaymentId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentTerm_DeletedAt",
                table: "PaymentTerms",
                newName: "IX_PaymentTerms_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_DeletedAt",
                table: "Payments",
                newName: "IX_Payments_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_ContractId",
                table: "Payments",
                newName: "IX_Payments_ContractId");

            migrationBuilder.RenameIndex(
                name: "IX_ContractDetail_ServicePackId",
                table: "ContractDetails",
                newName: "IX_ContractDetails_ServicePackId");

            migrationBuilder.RenameIndex(
                name: "IX_ContractDetail_DeletedAt",
                table: "ContractDetails",
                newName: "IX_ContractDetails_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_ContractDetail_ContractId",
                table: "ContractDetails",
                newName: "IX_ContractDetails_ContractId");

            migrationBuilder.RenameIndex(
                name: "IX_Contract_TeamId",
                table: "Contracts",
                newName: "IX_Contracts_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_Contract_DeletedAt",
                table: "Contracts",
                newName: "IX_Contracts_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Contract_CompanyId",
                table: "Contracts",
                newName: "IX_Contracts_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyMember_DeletedAt",
                table: "CompanyMembers",
                newName: "IX_CompanyMembers_DeletedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServicePacks",
                table: "ServicePacks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Services",
                table: "Services",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentTerms",
                table: "PaymentTerms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContractDetails",
                table: "ContractDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyMembers",
                table: "CompanyMembers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Configurations_DeletedAt",
                table: "Configurations",
                column: "DeletedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_CompanyMembers_CompanyMemberId",
                table: "Companies",
                column: "CompanyMemberId",
                principalTable: "CompanyMembers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractDetails_Contracts_ContractId",
                table: "ContractDetails",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractDetails_ServicePacks_ServicePackId",
                table: "ContractDetails",
                column: "ServicePackId",
                principalTable: "ServicePacks",
                principalColumn: "Id");

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
                name: "FK_PaymentTerms_Payments_PaymentId",
                table: "PaymentTerms",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_CompanyMembers_CompanyMemberId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractDetails_Contracts_ContractId",
                table: "ContractDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractDetails_ServicePacks_ServicePackId",
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
                name: "FK_PaymentTerms_Payments_PaymentId",
                table: "PaymentTerms");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_ContractDetails_ContractDetailId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServicePacks_ServicePackId",
                table: "Services");

            migrationBuilder.DropTable(
                name: "Configurations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Services",
                table: "Services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServicePacks",
                table: "ServicePacks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentTerms",
                table: "PaymentTerms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContractDetails",
                table: "ContractDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyMembers",
                table: "CompanyMembers");

            migrationBuilder.RenameTable(
                name: "Services",
                newName: "Service");

            migrationBuilder.RenameTable(
                name: "ServicePacks",
                newName: "ServicePack");

            migrationBuilder.RenameTable(
                name: "PaymentTerms",
                newName: "PaymentTerm");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payment");

            migrationBuilder.RenameTable(
                name: "Contracts",
                newName: "Contract");

            migrationBuilder.RenameTable(
                name: "ContractDetails",
                newName: "ContractDetail");

            migrationBuilder.RenameTable(
                name: "CompanyMembers",
                newName: "CompanyMember");

            migrationBuilder.RenameIndex(
                name: "IX_Services_ServicePackId",
                table: "Service",
                newName: "IX_Service_ServicePackId");

            migrationBuilder.RenameIndex(
                name: "IX_Services_DeletedAt",
                table: "Service",
                newName: "IX_Service_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Services_ContractDetailId",
                table: "Service",
                newName: "IX_Service_ContractDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_ServicePacks_DeletedAt",
                table: "ServicePack",
                newName: "IX_ServicePack_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentTerms_PaymentId",
                table: "PaymentTerm",
                newName: "IX_PaymentTerm_PaymentId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentTerms_DeletedAt",
                table: "PaymentTerm",
                newName: "IX_PaymentTerm_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_DeletedAt",
                table: "Payment",
                newName: "IX_Payment_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_ContractId",
                table: "Payment",
                newName: "IX_Payment_ContractId");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_TeamId",
                table: "Contract",
                newName: "IX_Contract_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_DeletedAt",
                table: "Contract",
                newName: "IX_Contract_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_CompanyId",
                table: "Contract",
                newName: "IX_Contract_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_ContractDetails_ServicePackId",
                table: "ContractDetail",
                newName: "IX_ContractDetail_ServicePackId");

            migrationBuilder.RenameIndex(
                name: "IX_ContractDetails_DeletedAt",
                table: "ContractDetail",
                newName: "IX_ContractDetail_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_ContractDetails_ContractId",
                table: "ContractDetail",
                newName: "IX_ContractDetail_ContractId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyMembers_DeletedAt",
                table: "CompanyMember",
                newName: "IX_CompanyMember_DeletedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Service",
                table: "Service",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServicePack",
                table: "ServicePack",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentTerm",
                table: "PaymentTerm",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contract",
                table: "Contract",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContractDetail",
                table: "ContractDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyMember",
                table: "CompanyMember",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_CompanyMember_CompanyMemberId",
                table: "Companies",
                column: "CompanyMemberId",
                principalTable: "CompanyMember",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Companies_CompanyId",
                table: "Contract",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Teams_TeamId",
                table: "Contract",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractDetail_Contract_ContractId",
                table: "ContractDetail",
                column: "ContractId",
                principalTable: "Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractDetail_ServicePack_ServicePackId",
                table: "ContractDetail",
                column: "ServicePackId",
                principalTable: "ServicePack",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Contract_ContractId",
                table: "Payment",
                column: "ContractId",
                principalTable: "Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTerm_Payment_PaymentId",
                table: "PaymentTerm",
                column: "PaymentId",
                principalTable: "Payment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_ContractDetail_ContractDetailId",
                table: "Service",
                column: "ContractDetailId",
                principalTable: "ContractDetail",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_ServicePack_ServicePackId",
                table: "Service",
                column: "ServicePackId",
                principalTable: "ServicePack",
                principalColumn: "Id");
        }
    }
}
