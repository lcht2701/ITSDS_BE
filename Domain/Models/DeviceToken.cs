using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class DeviceToken : BaseEntity
    {
        public int? UserId { get; set; }

        public string? Token { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }
    }
}
