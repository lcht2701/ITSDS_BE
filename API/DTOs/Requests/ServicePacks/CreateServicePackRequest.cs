using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Requests.ServicePacks
{
    public class CreateServicePackRequest : IMapTo<ServicePack>
    {
        public ICollection<Service> Services { get; set; }
        public string Description { get; set; }
    }
}
