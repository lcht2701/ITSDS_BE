using System.ComponentModel;

namespace Domain.Constants.Enums;

public enum Urgency
{
    [Description("Low")]
    Low,
    [Description("Middle")]
    Middle,
    [Description("High")]
    High,
    [Description("Urgent")]
    Urgent,
}
