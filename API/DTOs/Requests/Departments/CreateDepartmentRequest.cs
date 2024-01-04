using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Requests.Departments
{
    public class CreateDepartmentRequest : IMapTo<Department>
    {
        public int CompanyId { get; set; }

        public string Address { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
