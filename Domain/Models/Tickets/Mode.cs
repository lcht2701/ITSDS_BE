﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Tickets
{
    public class Mode : BaseEntity
    {
        public Mode()
        {
            Tickets = new HashSet<Ticket>();
        }

        public string? Name { get; set; }

        public string? Description { get; set; }

        [JsonIgnore]
        public virtual ICollection<Ticket>? Tickets { get; set; }
    }
}
