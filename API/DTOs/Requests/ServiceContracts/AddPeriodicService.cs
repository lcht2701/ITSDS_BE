using API.Mappings;
using Domain.Models.Contracts;

namespace API.DTOs.Requests.ServiceContracts;

public class AddPeriodicService : IMapTo<ServiceContract>
{
    public int? ServiceId { get; set; }
    
    public int? Frequency {  get; set; } 
        
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}