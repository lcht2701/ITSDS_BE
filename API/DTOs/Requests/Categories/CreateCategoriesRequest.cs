using API.Mappings;
using Domain.Models.Contracts;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Categories
{
    public class CreateCategoriesRequest : IMapTo<Category>
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        public int? AssignedTechnicalId { get; set; }
    }
}
