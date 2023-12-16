using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.ServiceContracts
{
    public class ModifyServicesInContract
    {
        public int ContractId { get; set; }

        public List<int>? ServiceIds { get; set; }
    }
}
