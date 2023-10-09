using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Feedbacks
{
    public class UpdateFeedbackRequest
    {
        public string? Comment { get; set; }

        public bool? IsPublic { get; set; }
    }
}
