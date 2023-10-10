using System.ComponentModel;

namespace Domain.Constants.Enums;

public enum Priority
{
    [Description("Critical")]
    Critical,
    [Description("High")]
    High,
    [Description("Medium")]
    Medium,
    [Description("Normal")]
    Normal,
    [Description("Low")]
    Low,
}