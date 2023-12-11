using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Messagings
{
    public class SendNotificationRequest
    {
        [Required]
        public string Message { get; set; }
    }
}
