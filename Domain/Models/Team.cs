using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Models.Contracts;
using Domain.Models.Tickets;

namespace Domain.Models;

public partial class Team : BaseEntity
{
    public Team()
    {
        Users = new HashSet<User>();
        Assignments = new HashSet<Assignment>();
        Contracts = new HashSet<Contract>();
    }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public int ManagerId { get; set; }

    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; }

    [JsonIgnore]
    public virtual ICollection<Assignment> Assignments { get; set; }

    [JsonIgnore]
    public virtual ICollection<Contract> Contracts { get; set; }

}
