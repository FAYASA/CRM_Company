using System;

namespace seashore_CRM.Models.Entities
{
    public enum LeadHistoryType
    {
        Created,
        StatusChange,
        AssignmentChange,
        Qualification,
        Converted,
        ActivityLinked,
        Note,
        Custom
    }

    public class LeadHistory : BaseEntity
    {
        public int LeadId { get; set; }

        public LeadHistoryType Type { get; set; }

        // Optional: field name changed (e.g., "StatusId", "AssignedUserId")
        public string? FieldName { get; set; }

        // Generic old/new values (stringified)
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        // Denormalized status snapshot (preferred for status changes)
        public int? OldStatusId { get; set; }
        public string? OldStatusName { get; set; }
        public int? NewStatusId { get; set; }
        public string? NewStatusName { get; set; }

        public int? ChangedById { get; set; }

        // Timestamp of the change (UTC)
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        // Optionally link to a LeadStatusActivity entry
        public int? RelatedLeadStatusActivityId { get; set; }

        public string? Note { get; set; }

        // Navigation
        public Lead? Lead { get; set; }
        public User? ChangedBy { get; set; }
        public LeadStatusActivity? RelatedLeadStatusActivity { get; set; }
    }
}
