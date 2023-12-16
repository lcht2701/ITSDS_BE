using API.Mappings;
using Domain.Models;
using Domain.Models.Tickets;

namespace API.DTOs.Responses.TicketSolutions;

public class GetTicketSolutionResponse : IMapFrom<TicketSolution>
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public int? CategoryId { get; set; }

    public int? OwnerId { get; set; }

    public int? CreatedById { get; set; }

    public DateTime? ReviewDate { get; set; }

    public DateTime? ExpiredDate { get; set; }

    public string? Keyword { get; set; }

    public string? InternalComments { get; set; }

    public List<string>? AttachmentUrls { get; set; }

    public bool? IsApproved { get; set; }

    public bool? IsPublic { get; set; }

    public int? CountLike { get; set; }

    public int? CountDislike { get; set; }

    public int? CurrentReactionUser { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual User? Owner { get; set; }

    public virtual User? CreatedBy { get; set; }

    public virtual Category? Category { get; set; }
}
