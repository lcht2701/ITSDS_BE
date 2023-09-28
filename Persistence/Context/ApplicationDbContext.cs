using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Domain.Models.Tickets;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Domain.Models.Contracts;

namespace Persistence.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<Configuration> Configurations { get; set; }
        //Ticket
        public virtual DbSet<Assignment> Assignments { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<TicketApproval> TicketApprovals { get; set; }
        public virtual DbSet<TicketTask> TicketTasks { get; set; }
        //Contract
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CompanyMember> CompanyMembers { get; set; }
        public virtual DbSet<Contract> Contracts { get; set; }
        public virtual DbSet<ContractDetail> ContractDetails { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentTerm> PaymentTerms { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServicePack> ServicePacks { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                //other automated configurations left out
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    entityType.AddSoftDeleteQueryFilter();
                }
            }

        }

        #region Add Convention for DateOnly & TimeOnly
        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            base.ConfigureConventions(builder);
            builder.Properties<DateOnly>()
                .HaveConversion<DateOnlyConverter>();
            builder.Properties<TimeOnly>()
                .HaveConversion<TimeOnlyConverter>();
        }
        #endregion

        #region DateOnly & TimeOnly Converter
        public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
        {
            public DateOnlyConverter() : base(
                dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
                dateTime => DateOnly.FromDateTime(dateTime))
            { }
        }

        public class TimeOnlyConverter : ValueConverter<TimeOnly, TimeSpan>
        {
            public TimeOnlyConverter() : base(
                timeOnly => timeOnly.ToTimeSpan(),
                timeSpan => TimeOnly.FromTimeSpan(timeSpan))
            { }
        }
        #endregion
    }
}
