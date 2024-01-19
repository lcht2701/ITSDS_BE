namespace API.DTOs.Requests.Messagings
{
    public class SendChatNotificationRequest
    {
        public string Email { get; set; }

        public string Type { get; set; }

        public string Content { get; set; }
    }
}
