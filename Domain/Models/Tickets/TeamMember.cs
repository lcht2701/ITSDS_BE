namespace Domain.Models.Tickets
{
    public partial class TeamMember : BaseEntity
    {
        public int MemberId { get; set; }

        public int TeamId { get; set; }

        public string? Expertises { get; set; }

        public virtual User? Member { get; set; }

        public virtual Team? Team { get; set; }
    }
}
