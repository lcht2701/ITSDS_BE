using API.Mappings;
using Domain.Models.Contracts;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Contracts
{
    public class UpdateContractRequest : IMapTo<Contract>
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public double? Value { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? ParentContractId { get; set; }

        public int? AccountantId { get; set; }

        public int CompanyId { get; set; }

        public string? AttachmentURl { get; set; }
    }
}
