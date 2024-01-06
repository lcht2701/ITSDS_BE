using Domain.Constants.Enums;
using Domain.Models.Contracts;

namespace Persistence.Helpers
{
    public static class CommonService
    {
        public static string CreateRandomPassword()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            Random random = new Random();
            string randomPassword = new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());
            return randomPassword;
        }
        public static void SetContractStatus(Contract entity)
        {
            if (DateTime.Today < entity.StartDate)
            {
                entity.Status = ContractStatus.Pending;
            }
            else if (DateTime.Today >= entity.StartDate && DateTime.Today <= entity.EndDate)
            {
                entity.Status = ContractStatus.Active;
            }
            else
            {
                entity.Status = ContractStatus.Expired;
            }
        }
    }
}
