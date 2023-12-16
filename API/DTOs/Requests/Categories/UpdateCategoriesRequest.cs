using API.Mappings;
using Domain.Models.Tickets;

namespace API.DTOs.Requests.Categories
{
    public class UpdateCategoriesRequest : IMapTo<Category>
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public int? AssignedTechnicalId { get; set; }

    }
}
