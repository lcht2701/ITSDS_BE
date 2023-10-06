using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Tickets
{
    public class TicketAnalyst : BaseEntity
    {
        public int? TicketId { get; set; }

        public string? Impact { get; set; }

        public string? RootCause { get; set; }

        public string? Symptoms { get; set; }

        public string? Attachments { get; set; }

        public string? Solution { get; set; }

        [JsonIgnore]
        public virtual Ticket? Ticket { get; set; }
    }
}
