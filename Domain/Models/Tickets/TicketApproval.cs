using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Tickets;

public partial class TicketApproval : BaseEntity
{
    public int CreatedBy { get; set; }

    public string Description { get; set; }

    public int ApprovalStatus { get; set; }

    public DateTime ApprovalDate { get; set; }

    public string ApprovalReason { get; set; }

    public DateTime EndDate { get; set; }
}
