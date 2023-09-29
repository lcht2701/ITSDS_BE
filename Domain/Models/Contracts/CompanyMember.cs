using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Contracts
{
    public partial class CompanyMember :  BaseEntity
    {
        public int MemberId { get; set; }

        public string MemberPosition { get; set; }
    }
}
