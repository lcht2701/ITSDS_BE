using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Requests.Departments
{
    public class UpdateDepartmentRequest : IMapTo<Department>
    {
        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
