using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Requests.CompanyMembers
{
    public class UpdateCompanyMemberRequest : IMapTo<CompanyMember>
    {
        public bool? IsCompanyAdmin { get; set; }

        public int? CompanyAddressId { get; set; }

        public string? MemberPosition { get; set; }
    }
}
