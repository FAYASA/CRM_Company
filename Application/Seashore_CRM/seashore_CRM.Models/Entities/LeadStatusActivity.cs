using System;

namespace seashore_CRM.Models.Entities
{
    public class LeadStatusActivity : BaseEntity
    {
        // Optional link to a specific lead (used when an activity is recorded against a lead)
        public int? LeadId { get; set; }
        public int LeadStatusId { get; set; }
        public string ActivityName { get; set; } = null!;

        public DateTime ActivityDate { get; set; } = DateTime.UtcNow;
        public DateTime? NextFollowUpDate { get; set; }
        public int? CreatedById { get; set; }

        public LeadStatus? LeadStatus { get; set; }

        public Lead? Lead { get; set; }
    }
} 