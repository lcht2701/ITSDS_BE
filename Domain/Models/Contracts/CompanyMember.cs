﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Contracts
{
    public partial class CompanyMember :  BaseEntity
    {
        public string MemberPosition { get; set; }

        public int MemberId { get; set; }

        public virtual User Member { get; set; }
    }
}