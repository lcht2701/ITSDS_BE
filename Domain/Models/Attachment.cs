namespace Domain.Models
{
    public class Attachment : BaseEntity
    {
        public string Url { get; set; }

        public string Table { get; set; }

        public int RowId { get; set; }
    }
}
