using System.ComponentModel;

namespace Domain.Constants.Enums;

public enum TicketTaskStatus
{
    [Description("Open")]
    Open,
    [Description("Assigned")]
    Assigned,
    [Description("In Progress")]
    InProgress,
    [Description("On Hold")]
    OnHold,
    [Description("Resolved")]
    Resolved,
    [Description("Closed")]
    Closed,
    [Description("Cancelled")]
    Cancelled
}