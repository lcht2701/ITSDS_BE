using System.ComponentModel;

namespace Domain.Constants.Enums;

public enum Role
{
    [Description("Admin")]
    Admin,
    [Description("Customer")]
    Customer,
    [Description("Manager")]
    Manager,
    [Description("Technician")]
    Technician,
    [Description("Accountant")]
    Accountant,
}
