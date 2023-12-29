using API.Mappings;
using Domain.Models.Contracts;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Categories
{
    public class CreateCategoriesRequest : IMapTo<Category>
    {
        public string? Name { get; set; }

        public string? Description { get; set; }
    }
}
