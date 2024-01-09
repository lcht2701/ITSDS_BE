using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Domain.Entities.Customs.CustomAttributes;

namespace Domain.Models;

public abstract class BaseEntity
{
    
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    [ExcludeFromAuditLog]
    public DateTime? CreatedAt { get; set; }

    [ExcludeFromAuditLog]
    public DateTime? ModifiedAt { get; set; }

    [ExcludeFromAuditLog]
    public DateTime? DeletedAt { get; set; }
}
