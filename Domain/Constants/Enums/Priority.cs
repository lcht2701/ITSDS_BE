using System.ComponentModel;

namespace Domain.Constants.Enums;

public enum Priority
{
    [Description("Low")]
    Low,
    [Description("Medium")]
    Medium,
    [Description("High")]
    High,
    [Description("Critical")]
    Critical,
}