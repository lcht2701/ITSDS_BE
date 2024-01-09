namespace Domain.Entities.Customs
{
    public class CustomAttributes
    {
        [AttributeUsage(AttributeTargets.Property)]
        public class ExcludeFromAuditLogAttribute : Attribute
        {
        }
    }
}
