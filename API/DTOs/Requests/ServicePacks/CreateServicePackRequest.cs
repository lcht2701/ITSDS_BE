using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Requests.ServicePacks
{
    public class CreateServicePackRequest : IMapTo<ServicePack>
    {
        public string? Description { get; set; }
    }
}
