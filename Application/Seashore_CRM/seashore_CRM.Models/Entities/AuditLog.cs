using System;

namespace seashore_CRM.Models.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string? KeyValues { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string Action { get; set; } = string.Empty; // Insert, Update, Delete
        public string? ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public string? CorrelationId { get; set; }
    }
}