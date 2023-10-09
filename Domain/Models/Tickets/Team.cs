using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Models.Contracts;

namespace Domain.Models.Tickets;

public partial class Team : BaseEntity
{
    public Team()
    {
        Assignments = new HashSet<Assignment>();
        Contracts = new HashSet<Contract>();
    }

    public string? Name { get; set; }

    public string? Location { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public int? ManagerId { get; set; }

    [JsonIgnore]
    public virtual User? Manager { get; set; }

    [JsonIgnore]
    public virtual ICollection<Contract>? Contracts { get; set; }
    [JsonIgnore]
    public virtual ICollection<Assignment>? Assignments { get; set; }
}
