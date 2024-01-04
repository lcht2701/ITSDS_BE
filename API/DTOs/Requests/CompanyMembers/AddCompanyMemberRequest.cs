﻿using System.ComponentModel;

namespace API.DTOs.Requests.CompanyMembers
{
    public class AddCompanyMemberRequest
    {
        public AddAccountInformationRequest User { get; set; }

        [DefaultValue(false)]
        public bool IsCompanyAdmin { get; set; }

        public string? MemberPosition { get; set; }

        public int DepartmentId { get; set; }
    }
}
