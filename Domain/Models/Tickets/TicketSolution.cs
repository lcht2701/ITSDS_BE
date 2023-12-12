using System.Text.Json.Serialization;

namespace Domain.Models.Tickets
{
    public class TicketSolution : BaseEntity
    {
        public TicketSolution()
        {
            Feedbacks = new HashSet<Feedback>();
            Reactions = new HashSet<Reaction>();
        }

        public string Title { get; set; }

        public string? Content { get; set; }

        public int CategoryId { get; set; }

        public int? OwnerId { get; set; }

        public int? CreatedById { get; set; }

        public DateTime? ReviewDate { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public string? Keyword { get; set; }

        public string? InternalComments { get; set; }

        public bool IsApproved { get; set; }

        public bool IsPublic { get; set; }

        public string? AttachmentUrl { get; set; }

        public virtual User? Owner { get; set; }

        public virtual User? CreatedBy { get; set; }

        public virtual Category? Category { get; set; }

        [JsonIgnore]
        public virtual ICollection<Feedback>? Feedbacks { get; set; }

        [JsonIgnore]
        public virtual ICollection<Reaction>? Reactions { get; set; }
    }
}
