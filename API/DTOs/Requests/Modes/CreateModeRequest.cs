using API.Mappings;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Modes
{
    public class CreateModeRequest : IMapTo<Mode>
    {
        public string? Name { get; set; }

        public string? Description { get; set; }
    }
}
