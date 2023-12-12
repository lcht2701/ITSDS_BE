using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Tickets
{
    public partial class Reaction : BaseEntity
    {
        public int SolutionId { get; set; }

        public int UserId { get; set;}

        public int ReactionType {get; set;}

        public virtual TicketSolution? Solution { get; set; }
        
        public virtual User? User { get; set; }
    }
}
