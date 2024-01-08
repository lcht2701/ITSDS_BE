using API.Mappings;
using Domain.Models.Tickets;
using System.ComponentModel;

namespace API.DTOs.Requests.Feedbacks;


public class CreateFeedbackRequest : IMapTo<Feedback>
{
    public int? SolutionId { get; set; }

    public string? Comment { get; set; }

    [DefaultValue(false)]
    public bool? IsPublic { get; set; }
}

