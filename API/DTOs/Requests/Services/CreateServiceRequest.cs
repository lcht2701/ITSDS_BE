using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Requests.Services
{
    public class CreateServiceRequest : IMapTo<Service>
    {
        public string Description { get; set; }

        public int? CategoryId { get; set; }
    }
}
