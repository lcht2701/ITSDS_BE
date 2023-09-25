using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Tickets;

public partial class Team : BaseEntity
{
    public Team()
    {
        Users = new HashSet<User>();
        Assignments = new HashSet<Assignment>();
    }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public Guid ManagerId { get; set; }

    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; }

    [JsonIgnore]
    public virtual ICollection<Assignment> Assignments { get; set; }

}
