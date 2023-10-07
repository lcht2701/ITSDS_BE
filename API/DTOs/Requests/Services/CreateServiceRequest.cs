using API.Mappings;
using Domain.Models.Contracts;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Services
{
    public class CreateServiceRequest : IMapTo<Service>
    {
        [Required(ErrorMessage = "Type code is required")]
        public string Type { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        public string Amount { get; set; }
    }
}
