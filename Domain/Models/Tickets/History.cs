using Domain.Constants.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Tickets
{
    public class History : BaseEntity
    {
        public int? UserId { get; set; }

        public int? TicketId { get; set; }

        public string? Action { get; set; }

        public virtual Ticket? Ticket { get; set; }

    }
}
