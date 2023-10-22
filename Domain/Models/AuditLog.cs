using Domain.Constants.Enums;
using Domain.Models.Tickets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class AuditLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string? EntityName { get; set; }

        public int? EntityRowId { get; set; }

        public string? Action { get; set; }

        public string? Message { get; set; }

        public DateTime? Timestamp { get; set; }

        public virtual User? User { get; set; }

    }
}
