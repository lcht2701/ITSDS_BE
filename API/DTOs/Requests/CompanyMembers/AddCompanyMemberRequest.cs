using API.Mappings;
using Domain.Models.Contracts;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.CompanyMembers
{
    public class AddCompanyMemberRequest : IMapTo<CompanyMember>
    {
        public int? MemberId { get; set; }

        public int? CompanyId { get; set; }

        public string? MemberPosition { get; set; }
    }
}
