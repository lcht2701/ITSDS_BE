namespace Domain.Models.Contracts
{
    public partial class ServiceServicePack : BaseEntity
    {
        public ServiceServicePack() { }

        public int? ServicePackId { get; set; }

        public int? ServiceId { get; set; }

        public virtual ServicePack? ServicePack { get; set; }

        public virtual Service? Service { get; set; }
    }
}
