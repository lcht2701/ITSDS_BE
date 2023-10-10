using System.ComponentModel;

namespace Domain.Constants.Enums;

public enum Gender
{
    [Description("Male")]
    Male,
    [Description("Female")]
    Female,
    [Description("Other")]
    Other,
    [Description("Prefer Not To Say")]
    PreferNotToSay
}
