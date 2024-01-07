using API.Mappings;
using Domain.Models;
using Domain.Models.Contracts;

namespace API.DTOs.Responses.CompanyMembers
{
    public class GetCompanyMemberResponse : IMapFrom<CompanyMember>
    {
        public string? MemberPosition { get; set; }

        public bool IsCompanyAdmin { get; set; }

        public virtual User? Member { get; set; }

        public virtual Company? Company { get; set; }

        public CompanyAddress? CompanyAddress { get; set; }
    }
}
