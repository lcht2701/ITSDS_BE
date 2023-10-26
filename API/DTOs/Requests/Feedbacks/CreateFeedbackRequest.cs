using API.Mappings;
using Domain.Models.Tickets;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Feedbacks
{
    public class CreateFeedbackRequest : IMapTo<Feedback>
    {
        [Required]
        public int? SolutionId { get; set; }

        [Required]
        public string? Comment { get; set; }

        [DefaultValue(false)]
        public bool? IsPublic { get; set; }
    }
}
