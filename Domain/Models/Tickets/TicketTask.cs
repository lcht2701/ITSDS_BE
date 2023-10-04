using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Tickets;

public partial class TicketTask : BaseEntity
{
    public int TechnicianId { get; set; }

    public int TicketId { get; set; }

    public string Title { get; set; }

    public string? Description { get; set; }

    public string? Note { get; set; }

    public bool isDone { get; set; }

    public TaskStatus TaskStatus { get; set; }

    public DateTime? ScheduledStartTime { get; set; }

    public DateTime? ScheduledEndTime { get; set; }

    public int? TimeSpent { get; set; }

    public DateTime? DateCompleted { get; set; }

    [JsonIgnore]
    public virtual Ticket Ticket { get; set; }
    [JsonIgnore]
    public virtual User Technician { get; set; }
}
