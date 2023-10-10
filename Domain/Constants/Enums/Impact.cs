using System.ComponentModel;

namespace Domain.Constants.Enums;

public enum Impact
{
    [Description("Not Specified")]
    NotSpecified,
    [Description("High")]
    High,
    [Description("Middle")]
    Middle,
    [Description("Low")]
    Low
}
