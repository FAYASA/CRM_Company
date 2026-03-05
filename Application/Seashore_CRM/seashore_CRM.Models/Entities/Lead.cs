using System;

namespace seashore_CRM.Models.Entities
{
    public class Lead : BaseEntity
    {
        public string LeadType { get; set; } = null!; // Corporate / Individual  // done

        public int? CompanyId { get; set; } // done
        public int? ContactId { get; set; } // done

        public int? SourceId { get; set; } // done
        public int? StatusId { get; set; } // done

        public string? Priority { get; set; } // done
        public int? AssignedUserId { get; set; } // done 

        public DateTime? ExpectedClosureDate { get; set; } // done
        public DateTime? FollowUpDate { get; set; }

        // Qualification fields
        public bool IsQualified { get; set; } = false;
        public DateTime? QualifiedOn { get; set; }
        public int? QualifiedById { get; set; }
        public string? QualificationNotes { get; set; }

        //// Business/Opportunity related
        //public decimal? Budget { get; set; }
        //public DateTime? DecisionDate { get; set; }
        //public int? Probability { get; set; }

        public Company? Company { get; set; }
        public Contact? Contact { get; set; }
        public LeadSource? Source { get; set; }
        public LeadStatus? Status { get; set; }
        public User? AssignedUser { get; set; }
    }
}
