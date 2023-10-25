using API.Mappings;
using Domain.Models;
using Domain.Models.Tickets;

namespace API.DTOs.Responses.Feedbacks;

public class GetFeedbackCustomerResponse : IMapFrom<Feedback>
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? SolutionId { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual TicketSolution? TicketSolution { get; set; }

    public virtual User? User { get; set; }
}
