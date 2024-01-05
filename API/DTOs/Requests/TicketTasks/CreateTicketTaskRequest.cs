using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models.Tickets;
using System.ComponentModel;

namespace API.DTOs.Requests.TicketTasks
{
    public class CreateTicketTaskRequest : IMapTo<TicketTask>
    {
        public int? TicketId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Priority? Priority { get; set; }
        public DateTime? ScheduledStartTime { get; set; }
        public DateTime? ScheduledEndTime { get; set; }
        [DefaultValue(0)]
        public int? Progress { get; set; }
        public List<string>? AttachmentUrls { get; set; }
    }
}
