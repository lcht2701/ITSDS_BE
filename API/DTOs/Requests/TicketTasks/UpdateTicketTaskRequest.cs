using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models.Tickets;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.TicketTasks
{
    public class UpdateTicketTaskRequest : IMapTo<TicketTask>
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TicketTaskStatus? TaskStatus { get; set; }
        public Priority? Priority { get; set; }
        public DateTime? ScheduledStartTime { get; set; }
        public DateTime? ScheduledEndTime { get; set; }
        public int? Progress { get; set; }
        public List<string>? AttachmentUrls { get; set; }
        public string? Note { get; set; }
    }
}
