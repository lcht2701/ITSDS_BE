using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models;
using Domain.Models.Tickets;

namespace API.DTOs.Responses.TicketTasks
{
    public class GetTicketTaskResponse : IMapFrom<TicketTask>
    {
        public int? Id { get; set; }
        public int? TicketId { get; set; }
        public int? CreateById { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public Priority? Priority { get; set; }
        public DateTime? ScheduledStartTime { get; set; }
        public DateTime? ScheduledEndTime { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public int? Progress { get; set; }
        public TicketTaskStatus? TaskStatus { get; set; }
        public List<string>? AttachmentUrls { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public virtual User? CreateBy { get; set; }
        public virtual Ticket? Ticket { get; set; }
    }
}
