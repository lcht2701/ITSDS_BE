using API.Mappings;
using Domain.Models.Contracts;
using System.ComponentModel;

namespace API.DTOs.Requests.Contracts
{
    public class UpdateContractRequest : IMapTo<Contract>
    {
        public string? ContractNumber { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public double? Value { get; set; }

        public DateTime StartDate { get; set; }

        public int Duration { get; set; }

        public int CompanyId { get; set; }

        public List<string> AttachmentUrls { get; set; }

        public string? Note { get; set; }
    }
}
