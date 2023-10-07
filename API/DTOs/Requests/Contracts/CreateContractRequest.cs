using API.Mappings;
using Domain.Constants;
using Domain.Models.Contracts;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Contracts
{
    public class CreateContractRequest : IMapTo<Contract>
    {
        
        [Required(ErrorMessage = "StartDate is required")]
        public DateTime StartDate { get; set; }

        public string AttachmentURl { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "InitialValue is required")]
        public double InitialValue { get; set; }


        [Required(ErrorMessage = "Team Id is required")]
        public int TeamId { get; set; }

        [Required(ErrorMessage = "Company Id is required")]
        public int CompanyId { get; set; }

    }
}
