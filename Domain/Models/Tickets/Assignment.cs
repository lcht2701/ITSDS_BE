using Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Tickets;

public partial class Assignment : BaseEntity
{
    public string Title { get; set; }

    public string Description { get; set; }

    public string? Note { get; set; }

    public int Status { get; set; }

    public Priority Priority { get; set; }

    public int TeamId { get; set; }

    public virtual Team Team { get; set; }

    public virtual Ticket Ticket { get; set; }

    
}
