﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Contracts
{
    public partial class Service : BaseEntity
    {
        public Service() { }

        public string Description { get; set; }

        public string Type { get; set; }

        public string Amount { get; set; }
    }
}
