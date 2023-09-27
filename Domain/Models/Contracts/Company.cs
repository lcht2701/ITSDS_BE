using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Contracts
{
    public partial class Company : BaseEntity
    {
        public Company()
        {
            CompanyMembers = new HashSet<User>();
        }

        public string? LogoUrl { get; set; }

        public bool isActive { get; set; }


        [JsonIgnore]
        public virtual ICollection<User> CompanyMembers { get; set; }
    }
}
