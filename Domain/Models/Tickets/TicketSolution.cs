using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Tickets
{
    public class TicketSolution : BaseEntity
    {
        public TicketSolution() { }

        public string? Title { get; set; }

        public string? Content { get; set; }


    }
}
