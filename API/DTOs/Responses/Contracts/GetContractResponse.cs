using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models;
using Domain.Models.Contracts;
using static Domain.Customs.CustomAttributes;

namespace API.DTOs.Responses.Contracts
{
    public class GetContractResponse : IMapFrom<Contract>
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public double? Value { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsRenewed { get; set; }

        public int? ParentContractId { get; set; }

        public int? AccountantId { get; set; }

        public int? CompanyId { get; set; }

        public List<string>? AttachmentUrls { get; set; }

        public ContractStatus? Status { get; set; }

        public virtual User? Accountant { get; set; }

        public virtual Company? Company { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
