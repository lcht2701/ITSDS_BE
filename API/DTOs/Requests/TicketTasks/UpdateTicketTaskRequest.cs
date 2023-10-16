using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models.Tickets;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.TicketTasks
{
    public class UpdateTicketTaskRequest : IMapTo<TicketTask>
    {
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public TicketTaskStatus? TaskStatus { get; set; }
        public int? TechnicianId { get; set; }
        public int? TeamId { get; set; }
        public Priority? Priority { get; set; }
        public DateTime? ScheduledStartTime { get; set; }
        public DateTime? ScheduledEndTime { get; set; }
        public int? Progress { get; set; }
        public string? AttachmentUrl { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public string? Note { get; set; }
    }
}
