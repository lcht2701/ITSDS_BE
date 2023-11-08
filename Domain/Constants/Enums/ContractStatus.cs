using System.ComponentModel;

namespace Domain.Constants.Enums
{
    public enum ContractStatus
    {
        [Description("Pending to be Active")]
        Pending,
        [Description("Active")]
        Active,
        [Description("Inactive")]
        Inactive,
        [Description("Expired")]
        Expired,
    }
}
