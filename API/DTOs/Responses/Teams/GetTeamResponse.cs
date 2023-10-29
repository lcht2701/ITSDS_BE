using API.Mappings;
using Domain.Models;
using Domain.Models.Tickets;
using System.Text.Json.Serialization;

namespace API.DTOs.Responses.Teams
{
    public class GetTeamResponse : IMapFrom<Team>
    {
        public string? Name { get; set; }

        public string? Location { get; set; }

        public string? Description { get; set; }

        public bool? IsActive { get; set; }

        public int? ManagerId { get; set; }

        public string? ManagerName => $"{Manager?.FirstName} {Manager?.LastName}";

        [JsonIgnore]
        public virtual User? Manager { get; set; }
    }
}
