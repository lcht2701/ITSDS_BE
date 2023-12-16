using API.Mappings;
using Domain.Models.Tickets;

namespace API.DTOs.Requests.Feedbacks
{
    public class UpdateFeedbackRequest : IMapTo<Feedback>
    {
        public string? Comment { get; set; }

        public bool? IsPublic { get; set; }
    }
}
