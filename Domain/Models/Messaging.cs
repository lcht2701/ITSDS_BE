using System.Text.Json.Serialization;

namespace Domain.Models;

public class Messaging : BaseEntity
{
    public string? Title { get; set; }

    public string? Body { get; set; }

    public int? UserId { get; set; }

    public bool? IsRead { get; set; }

    [JsonIgnore]
    public virtual User? User { get; set; }

}
