﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Persistence.Context;

#nullable disable

namespace Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20231007122830_fix all datetime")]
    partial class fixalldatetime
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.22")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Domain.Models.Configuration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DeletedAt");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("Domain.Models.Contracts.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("CompanyAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CompanyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CustomerAdminId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FieldOfBusiness")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LogoUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TaxCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Website")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("isActive")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("CustomerAdminId");

                    b.HasIndex("DeletedAt");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("Domain.Models.Contracts.CompanyMember", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("CompanyId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("MemberId")
                        .HasColumnType("int");

                    b.Property<string>("MemberPosition")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("DeletedAt");

                    b.HasIndex("MemberId");

                    b.ToTable("CompanyMembers");
                });

            modelBuilder.Entity("Domain.Models.Contracts.Contract", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("AccountantId")
                        .HasColumnType("int");

                    b.Property<string>("AttachmentURl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CompanyId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Duration")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<int?>("TeamId")
                        .HasColumnType("int");

                    b.Property<double?>("Value")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("DeletedAt");

                    b.HasIndex("TeamId");

                    b.ToTable("Contracts");
                });

            modelBuilder.Entity("Domain.Models.Contracts.ContractDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("AdditionalServiceId")
                        .HasColumnType("int");

                    b.Property<int?>("ContractId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ServicePackId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AdditionalServiceId");

                    b.HasIndex("ContractId");

                    b.HasIndex("DeletedAt");

                    b.HasIndex("ServicePackId");

                    b.ToTable("ContractDetails");
                });

            modelBuilder.Entity("Domain.Models.Contracts.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("ContractId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("PaymentEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("PaymentFinishTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("PaymentStart")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaymentStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentType")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ContractId");

                    b.HasIndex("DeletedAt");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("Domain.Models.Contracts.PaymentTerm", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("PaymentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DeletedAt");

                    b.HasIndex("PaymentId");

                    b.ToTable("PaymentTerms");
                });

            modelBuilder.Entity("Domain.Models.Contracts.Service", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Amount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ServicePackId")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DeletedAt");

                    b.HasIndex("ServicePackId");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("Domain.Models.Contracts.ServicePack", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Price")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DeletedAt");

                    b.ToTable("ServicePacks");
                });

            modelBuilder.Entity("Domain.Models.Tickets.Assignment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TeamId")
                        .HasColumnType("int");

                    b.Property<int?>("TechnicianId")
                        .HasColumnType("int");

                    b.Property<int?>("TicketId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DeletedAt");

                    b.HasIndex("TeamId");

                    b.HasIndex("TechnicianId");

                    b.HasIndex("TicketId");

                    b.ToTable("Assignments");
                });

            modelBuilder.Entity("Domain.Models.Tickets.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("AssignedTechnicalId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AssignedTechnicalId");

                    b.HasIndex("DeletedAt");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Domain.Models.Tickets.History", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TicketId")
                        .HasColumnType("int");

                    b.Property<int>("TicketStatus")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DeletedAt");

                    b.HasIndex("TicketId");

                    b.ToTable("Histories");
                });

            modelBuilder.Entity("Domain.Models.Tickets.Mode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DeletedAt");

                    b.ToTable("Modes");
                });

            modelBuilder.Entity("Domain.Models.Tickets.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("OwnerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DeletedAt");

                    b.HasIndex("OwnerId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Domain.Models.Tickets.TeamMember", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Expertises")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("MemberId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TeamId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DeletedAt");

                    b.HasIndex("MemberId");

                    b.HasIndex("TeamId");

                    b.ToTable("TeamMembers");
                });

            modelBuilder.Entity("Domain.Models.Tickets.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AttachmentUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CompletedTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DueTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Impact")
                        .HasColumnType("int");

                    b.Property<int?>("ModeId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Priority")
                        .HasColumnType("int");

                    b.Property<int?>("RequesterId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ScheduledEndTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ScheduledStartTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ServiceId")
                        .HasColumnType("int");

                    b.Property<int?>("TeamId")
                        .HasColumnType("int");

                    b.Property<int?>("TicketStatus")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Urgency")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("DeletedAt");

                    b.HasIndex("ModeId");

                    b.HasIndex("RequesterId");

                    b.HasIndex("ServiceId");

                    b.HasIndex("TeamId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("Domain.Models.Tickets.TicketAnalyst", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Attachments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Impact")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("RootCause")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Solution")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Symptoms")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TicketId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DeletedAt");

                    b.HasIndex("TicketId");

                    b.ToTable("TicketAnalysts");
                });

            modelBuilder.Entity("Domain.Models.Tickets.TicketTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("ActualEndTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ActualStartTime")
                        .HasColumnType("datetime2");

                    b.Property<double?>("AdditionalCost")
                        .HasColumnType("float");

                    b.Property<string>("AttachmentUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateCompleted")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Priority")
                        .HasColumnType("int");

                    b.Property<int?>("Progress")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ScheduledEndTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ScheduledStartTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TaskStatus")
                        .HasColumnType("int");

                    b.Property<int?>("TeamId")
                        .HasColumnType("int");

                    b.Property<int?>("TechnicianId")
                        .HasColumnType("int");

                    b.Property<int?>("TicketId")
                        .HasColumnType("int");

                    b.Property<int?>("TimeSpent")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DeletedAt");

                    b.HasIndex("TeamId");

                    b.HasIndex("TicketId");

                    b.ToTable("TicketTasks");
                });

            modelBuilder.Entity("Domain.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Gender")
                        .HasColumnType("int");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DeletedAt");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Domain.Models.Contracts.Company", b =>
                {
                    b.HasOne("Domain.Models.User", "CustomerAdmin")
                        .WithMany()
                        .HasForeignKey("CustomerAdminId");

                    b.Navigation("CustomerAdmin");
                });

            modelBuilder.Entity("Domain.Models.Contracts.CompanyMember", b =>
                {
                    b.HasOne("Domain.Models.Contracts.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");

                    b.HasOne("Domain.Models.User", "Member")
                        .WithMany()
                        .HasForeignKey("MemberId");

                    b.Navigation("Company");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("Domain.Models.Contracts.Contract", b =>
                {
                    b.HasOne("Domain.Models.Contracts.Company", "Company")
                        .WithMany("Contracts")
                        .HasForeignKey("CompanyId");

                    b.HasOne("Domain.Models.Tickets.Team", "Team")
                        .WithMany("Contracts")
                        .HasForeignKey("TeamId");

                    b.Navigation("Company");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("Domain.Models.Contracts.ContractDetail", b =>
                {
                    b.HasOne("Domain.Models.Contracts.Service", "AdditionalService")
                        .WithMany("ContractDetails")
                        .HasForeignKey("AdditionalServiceId");

                    b.HasOne("Domain.Models.Contracts.Contract", "Contract")
                        .WithMany("ContractDetails")
                        .HasForeignKey("ContractId");

                    b.HasOne("Domain.Models.Contracts.ServicePack", "ServicePack")
                        .WithMany("ContractDetails")
                        .HasForeignKey("ServicePackId");

                    b.Navigation("AdditionalService");

                    b.Navigation("Contract");

                    b.Navigation("ServicePack");
                });

            modelBuilder.Entity("Domain.Models.Contracts.Payment", b =>
                {
                    b.HasOne("Domain.Models.Contracts.Contract", "Contract")
                        .WithMany("Payments")
                        .HasForeignKey("ContractId");

                    b.Navigation("Contract");
                });

            modelBuilder.Entity("Domain.Models.Contracts.PaymentTerm", b =>
                {
                    b.HasOne("Domain.Models.Contracts.Payment", "Payment")
                        .WithMany("PaymentTerms")
                        .HasForeignKey("PaymentId");

                    b.Navigation("Payment");
                });

            modelBuilder.Entity("Domain.Models.Contracts.Service", b =>
                {
                    b.HasOne("Domain.Models.Contracts.ServicePack", "ServicePacks")
                        .WithMany("Services")
                        .HasForeignKey("ServicePackId");

                    b.Navigation("ServicePacks");
                });

            modelBuilder.Entity("Domain.Models.Tickets.Assignment", b =>
                {
                    b.HasOne("Domain.Models.Tickets.Team", "Team")
                        .WithMany("Assignments")
                        .HasForeignKey("TeamId");

                    b.HasOne("Domain.Models.User", "Technician")
                        .WithMany("Assignments")
                        .HasForeignKey("TechnicianId");

                    b.HasOne("Domain.Models.Tickets.Ticket", "Ticket")
                        .WithMany("Assignments")
                        .HasForeignKey("TicketId");

                    b.Navigation("Team");

                    b.Navigation("Technician");

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("Domain.Models.Tickets.Category", b =>
                {
                    b.HasOne("Domain.Models.User", "AssignedTechnical")
                        .WithMany()
                        .HasForeignKey("AssignedTechnicalId");

                    b.Navigation("AssignedTechnical");
                });

            modelBuilder.Entity("Domain.Models.Tickets.History", b =>
                {
                    b.HasOne("Domain.Models.Tickets.Ticket", "Ticket")
                        .WithMany("Histories")
                        .HasForeignKey("TicketId");

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("Domain.Models.Tickets.Team", b =>
                {
                    b.HasOne("Domain.Models.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Domain.Models.Tickets.TeamMember", b =>
                {
                    b.HasOne("Domain.Models.User", "Member")
                        .WithMany("TeamMembers")
                        .HasForeignKey("MemberId");

                    b.HasOne("Domain.Models.Tickets.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId");

                    b.Navigation("Member");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("Domain.Models.Tickets.Ticket", b =>
                {
                    b.HasOne("Domain.Models.Tickets.Category", "Category")
                        .WithMany("Tickets")
                        .HasForeignKey("CategoryId");

                    b.HasOne("Domain.Models.Tickets.Mode", "Mode")
                        .WithMany("Tickets")
                        .HasForeignKey("ModeId");

                    b.HasOne("Domain.Models.User", "Requester")
                        .WithMany("Tickets")
                        .HasForeignKey("RequesterId");

                    b.HasOne("Domain.Models.Contracts.Service", "Service")
                        .WithMany("Tickets")
                        .HasForeignKey("ServiceId");

                    b.HasOne("Domain.Models.Tickets.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId");

                    b.Navigation("Category");

                    b.Navigation("Mode");

                    b.Navigation("Requester");

                    b.Navigation("Service");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("Domain.Models.Tickets.TicketAnalyst", b =>
                {
                    b.HasOne("Domain.Models.Tickets.Ticket", "Ticket")
                        .WithMany("TicketAnalysts")
                        .HasForeignKey("TicketId");

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("Domain.Models.Tickets.TicketTask", b =>
                {
                    b.HasOne("Domain.Models.Tickets.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId");

                    b.HasOne("Domain.Models.Tickets.Ticket", "Ticket")
                        .WithMany("TicketTasks")
                        .HasForeignKey("TicketId");

                    b.Navigation("Team");

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("Domain.Models.Contracts.Company", b =>
                {
                    b.Navigation("Contracts");
                });

            modelBuilder.Entity("Domain.Models.Contracts.Contract", b =>
                {
                    b.Navigation("ContractDetails");

                    b.Navigation("Payments");
                });

            modelBuilder.Entity("Domain.Models.Contracts.Payment", b =>
                {
                    b.Navigation("PaymentTerms");
                });

            modelBuilder.Entity("Domain.Models.Contracts.Service", b =>
                {
                    b.Navigation("ContractDetails");

                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Domain.Models.Contracts.ServicePack", b =>
                {
                    b.Navigation("ContractDetails");

                    b.Navigation("Services");
                });

            modelBuilder.Entity("Domain.Models.Tickets.Category", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Domain.Models.Tickets.Mode", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Domain.Models.Tickets.Team", b =>
                {
                    b.Navigation("Assignments");

                    b.Navigation("Contracts");
                });

            modelBuilder.Entity("Domain.Models.Tickets.Ticket", b =>
                {
                    b.Navigation("Assignments");

                    b.Navigation("Histories");

                    b.Navigation("TicketAnalysts");

                    b.Navigation("TicketTasks");
                });

            modelBuilder.Entity("Domain.Models.User", b =>
                {
                    b.Navigation("Assignments");

                    b.Navigation("TeamMembers");

                    b.Navigation("Tickets");
                });
#pragma warning restore 612, 618
        }
    }
}
