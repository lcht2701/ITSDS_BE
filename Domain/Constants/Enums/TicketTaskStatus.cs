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
    [Description("Completed")]
    Completed,
    [Description("Cancelled")]
    Cancelled
}