using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models.Contracts;

namespace API.DTOs.Responses.Contracts
{
    public class GetContractResponse : IMapFrom<Contract>
    {
        public int Id { get; set; }

        public string? ContractNumber { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public double? Value { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? CompanyId { get; set; }

        public List<string>? AttachmentUrls { get; set; }

        public ContractStatus? Status { get; set; }

        public virtual Company? Company { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
