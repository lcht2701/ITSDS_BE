using API.Mappings;
using Domain.Models.Contracts;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.CompanyMembers
{
    public class AddCompanyMemberRequest : IMapTo<CompanyMember>
    {
        [Required]
        public int? MemberId { get; set; }

        [Required]
        public int? CompanyId { get; set; }

        public string? MemberPosition { get; set; }
    }
}
