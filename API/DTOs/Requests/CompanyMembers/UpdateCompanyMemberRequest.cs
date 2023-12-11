using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Requests.CompanyMembers
{
    public class UpdateCompanyMemberRequest : IMapTo<CompanyMember>
    {
        public string? MemberPosition { get; set; }
    }
}
