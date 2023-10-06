using API.Mappings;
using Domain.Constants;
using Domain.Models.Contracts;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Contracts
{
    public class UpdateContractRequest : IMapTo<Contract>
    {
        public string AttachmentURl { get; set; }

        [Required]
        public int Duration { get; set; }
        [Required]
        public double InitialValue { get; set; }


    }
}
