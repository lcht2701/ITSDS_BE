using API.Mappings;
using Domain.Models;
using Domain.Models.Contracts;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Services
{
    public class UpdateServiceRequest : IMapTo<Service>
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
    }
}
