using System.ComponentModel;

namespace Domain.Constants.Enums;

public enum Urgency
{
    [Description("Not Specified")]
    NotSpecified,
    [Description("Urgent")]
    Urgent,
    [Description("High")]
    High,
    [Description("Middle")]
    Middle,
    [Description("Low")]
    Low
}
