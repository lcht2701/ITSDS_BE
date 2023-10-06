using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Requests.ServicePacks
{
    public class UpdateServicePackRequest : IMapTo<ServicePack>
    {
        public string Description { get; set; }
    }
}
