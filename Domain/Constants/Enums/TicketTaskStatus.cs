using System.ComponentModel;

namespace Domain.Constants.Enums;

public enum TicketTaskStatus
{
    [Description("New")]
    New,
    [Description("In Progress")]
    InProgress,
    [Description("Completed")]
    Completed,
    [Description("Cancelled")]
    Cancelled
}