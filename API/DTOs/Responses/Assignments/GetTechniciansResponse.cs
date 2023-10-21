using API.Mappings;
using Domain.Models;
using System.Text.Json.Serialization;

namespace API.DTOs.Responses.Assignments
{
    public class GetTechniciansResponse : IMapFrom<User>
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Username { get; set; }
    }
}
