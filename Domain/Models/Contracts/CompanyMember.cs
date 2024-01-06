namespace Domain.Models.Contracts
{
    public partial class CompanyMember :  BaseEntity
    { 
        public int MemberId { get; set; }

        public int CompanyId { get; set; }

        public string? MemberPosition { get; set; }

        public bool IsCompanyAdmin { get; set; }

        public int? CompanyAddressId { get; set; }

        public virtual User? Member { get; set; }

        public virtual Company? Company { get; set; }

        public virtual CompanyAddress? CompanyAddress { get; set; }

    }
}
