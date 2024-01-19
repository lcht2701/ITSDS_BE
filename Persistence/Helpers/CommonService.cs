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
            DateTime today = DateTime.Today;

            if (today < entity.StartDate.Date)
            {
                entity.Status = ContractStatus.Pending;
            }
            else if (today >= entity.StartDate.Date && today <= entity.EndDate.Date)
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
