namespace API.DTOs.Requests.Notifications
{
    public class SendNotificationRequest
    {
        public string Body { get; set; }
        public string DeviceToken { get; set; }
    }
}
