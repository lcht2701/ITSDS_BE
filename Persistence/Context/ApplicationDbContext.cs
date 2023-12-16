using Domain.Models;
using Domain.Models.Contracts;
using Domain.Models.Tickets;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public virtual DbSet<User>? Users { get; set; }
        public virtual DbSet<AuditLog>? AuditLogs { get; set; }
        public virtual DbSet<Messaging>? Messagings { get; set; }
        public virtual DbSet<DeviceToken>? DeviceTokens { get; set; }
        public virtual DbSet<Attachment>? Attachments { get; set; }
        //Ticket
        public virtual DbSet<Team>? Teams { get; set; }
        public virtual DbSet<TeamMember>? TeamMembers { get; set; }
        public virtual DbSet<Assignment>? Assignments { get; set; }
        public virtual DbSet<Ticket>? Tickets { get; set; }
        public virtual DbSet<Mode>? Modes { get; set; }
        public virtual DbSet<Category>? Categories { get; set; }
        public virtual DbSet<TicketTask>? TicketTasks { get; set; }
        public virtual DbSet<TicketSolution>? TicketSolutions { get; set; }
        public virtual DbSet<Feedback>? Feedbacks { get; set; }
        //Contract
        public virtual DbSet<Company>? Companies { get; set; }
        public virtual DbSet<CompanyMember>? CompanyMembers { get; set; }
        public virtual DbSet<Renewal>? Renewals { get; set; }
        public virtual DbSet<Contract>? Contracts { get; set; }
        public virtual DbSet<ServiceContract>? ServiceContracts { get; set; }
        public virtual DbSet<Payment>? Payments { get; set; }
        public virtual DbSet<PaymentTerm>? PaymentTerms { get; set; }
        public virtual DbSet<Service>? Services { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TicketSolution>()
                    .HasOne(ts => ts.CreatedBy)
                    .WithMany()
                    .HasForeignKey(ts => ts.CreatedById);

            builder.Entity<Ticket>()
                .HasOne(t => t.CreatedBy)
                .WithMany()
                .HasForeignKey(ts => ts.CreatedById);

            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            
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
    }
}
