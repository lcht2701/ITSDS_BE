using System.Text.Json.Serialization;

namespace Domain.Models.Tickets;

public class Feedback : BaseEntity
{
    public int? UserId { get; set; }
    
    public int? SolutionId { get; set; }
    
    public string? Comment { get; set; }
    
    public bool? IsPublic { get; set; }

    public int? ParentFeedbackId { get; set; }
    
    public virtual TicketSolution? Solution { get; set; }

    public virtual User? User { get; set; }
}