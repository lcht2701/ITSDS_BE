using System.ComponentModel;

namespace Domain.Constants.Enums;

public enum TicketStatus
{
    [Description("Open")]
    Open,
    [Description("Assigned")]
    Assigned,
    [Description("In Progress")]
    InProgress,
    [Description("Resolved")]
    Resolved,
    [Description("Closed")]
    Closed,
    [Description("Cancelled")]
    Cancelled
}