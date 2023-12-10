using API.Mappings;
using Domain.Models.Contracts;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Contracts
{
    public class RenewContractRequest : IMapTo<Contract>
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public double? Value { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int AccountantId { get; set; }

        public List<string>? AttachmentURls { get; set; }
    }
}
