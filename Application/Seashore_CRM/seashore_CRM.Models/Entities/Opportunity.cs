using System;

namespace seashore_CRM.Models.Entities
{
    public class Opportunity : BaseEntity
    {
        public int LeadId { get; set; }
        public string Stage { get; set; } = null!;
        public decimal EstimatedValue { get; set; }
        public int Probability { get; set; }
        public DateTime? ExpectedCloseDate { get; set; }

        public Lead? Lead { get; set; }
    }
}
