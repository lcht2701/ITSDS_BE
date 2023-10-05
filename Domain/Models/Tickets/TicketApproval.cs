using Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Tickets;

public partial class TicketApproval : BaseEntity
{
    public int TicketId { get; set; }

    public int? ApprovalCreaterId { get; set; }

    public int? ApproverId { get; set; }

    public string Title { get; set; }

    public string? Description { get; set; }

    public bool? IsApproved { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? ApprovalDate { get; set; }

    public string? Comments { get; set; }

    public TicketStatus? Stage { get; set; }

    [JsonIgnore]
    public virtual Ticket Ticket { get; set; }
    [JsonIgnore]
    public virtual User ApprovalCreater { get; set; }
    [JsonIgnore]
    public virtual User Approver { get; set; }
}
