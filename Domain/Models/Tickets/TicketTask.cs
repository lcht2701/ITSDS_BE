using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Tickets;

public partial class TicketTask : BaseEntity
{
    public string Title { get; set; }

    public string Description { get; set; }

    public string Note { get; set; }

    public int TimeSpent { get; set; }

    public DateTime DateCompleted { get; set; }

    public string Reason { get; set; }

    public TaskStatus TaskStatus { get; set; }

    public bool isDone { get; set; }

    public int TicketId { get; set; }

    public int CreatedBy { get; set; }
}
