using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Tickets
{
    public partial class TeamMember : BaseEntity
    {
        public int? MemberId { get; set; }

        public int? TeamId { get; set; }

        public string? Expertises { get; set; }

        [JsonIgnore]
        public virtual User Member { get; set; }
        [JsonIgnore]
        public virtual Team Team { get; set; }
    }
}
