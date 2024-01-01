using API.Mappings;
using Domain.Models;

namespace API.DTOs.Requests.Messagings
{
    public class GetTokenRequest : IMapTo<Messaging>
    {
        public string Token { get; set; }
    }
}
