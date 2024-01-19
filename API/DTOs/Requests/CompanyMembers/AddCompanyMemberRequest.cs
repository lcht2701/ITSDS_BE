using System.ComponentModel;

namespace API.DTOs.Requests.CompanyMembers
{
    public class AddCompanyMemberRequest
    {
        public AddAccountInformationRequest User { get; set; }

        public string? MemberPosition { get; set; }

        public int CompanyAddressId { get; set; }
    }
}
