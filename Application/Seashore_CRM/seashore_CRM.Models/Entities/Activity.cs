using System;

namespace seashore_CRM.Models.Entities
{
    public class Activity : BaseEntity
    {
        public int? LeadId { get; set; }
        public int? CustomerId { get; set; }
        public string ActivityType { get; set; } = null!; // Call, Email, Meeting
        public string? Description { get; set; }
        public DateTime ActivityDate { get; set; } = DateTime.UtcNow;
        public DateTime? NextFollowUpDate { get; set; }
        public int? CreatedById { get; set; }

        public Lead? Lead { get; set; }
        public Company? Customer { get; set; }
        public User? CreatedBy { get; set; }
    }
}
