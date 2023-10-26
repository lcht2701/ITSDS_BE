using API.Mappings;
using Domain.Models.Tickets;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Feedbacks;

public class CreateFeedbackRequest : IMapTo<Feedback>
{
    [Required]
    public string? Comment { get; set; }

    [Required]
    [DefaultValue(false)]
    public bool? IsPublic { get; set; }
}
