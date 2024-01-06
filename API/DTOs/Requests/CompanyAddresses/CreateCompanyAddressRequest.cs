using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Requests.CompanyAddresss
{
    public class CreateCompanyAddressRequest : IMapTo<CompanyAddress>
    {
        public string Address { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
