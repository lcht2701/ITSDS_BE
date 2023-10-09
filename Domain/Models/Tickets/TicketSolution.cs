﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Tickets
{
    public class TicketSolution : BaseEntity
    {
        public TicketSolution()
        {
            Feedbacks = new HashSet<Feedback>();
        }

        public string? Title { get; set; }

        public string? Content { get; set; }
        
        public int? CategoryId { get; set; }
        
        public int? OwnerId { get; set; }
        
        public DateTime? ReviewDate { get; set; }
        
        public DateTime? ExpiredDate { get; set; }
        
        public string? Keyword { get; set; }
        
        public string? InternalComments { get; set; }
        
        public bool? IsApproved { get; set; }
        
        public bool? IsPublic { get; set; }
        
        [JsonIgnore]
        public virtual Category Category { get; set; }  
        [JsonIgnore]
        public virtual ICollection<Feedback>? Feedbacks { get; set; }  
    }
}
