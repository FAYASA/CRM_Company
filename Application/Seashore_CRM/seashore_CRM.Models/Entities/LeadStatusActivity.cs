using System;

namespace seashore_CRM.Models.Entities
{
    public class LeadStatusActivity : BaseEntity
    {
        public int LeadStatusId { get; set; }
        public string ActivityName { get; set; } = null!;

        public LeadStatus? LeadStatus { get; set; }
    }
}
