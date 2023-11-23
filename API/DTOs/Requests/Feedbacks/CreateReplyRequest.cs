using API.Mappings;
using Domain.Models.Tickets;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Feedbacks;


public class CreateReplyRequest : IMapTo<Feedback>
{
    public string? Comment { get; set; }

    public int? ParentFeedbackId { get; set; }

    [DefaultValue(false)]
    public bool? IsPublic { get; set; }
}

