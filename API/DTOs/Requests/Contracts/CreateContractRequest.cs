using API.Mappings;
using Domain.Models.Contracts;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Contracts
{
    public class CreateContractRequest : IMapTo<Contract>
    {
        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        public double? Value { get; set; }

        [Required(ErrorMessage = "StartDate is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "EndDate is required")]
        public DateTime EndDate { get; set; }

        public int? ParentContractId { get; set; }
        
        [Required(ErrorMessage = "An employee is required to manage the contract")]
        public int AccountantId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        public int CompanyId { get; set; }

        public string? AttachmentURl { get; set; }
    }
}
