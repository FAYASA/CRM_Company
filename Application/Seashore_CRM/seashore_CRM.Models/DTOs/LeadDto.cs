using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace seashore_CRM.Models.DTOs
{
    public class LeadDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Lead Type is required")]
        public string LeadType { get; set; } = null!;

        public int? CompanyId { get; set; }
        public int? ContactId { get; set; }

        public int? SourceId { get; set; }
        public int? StatusId { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public string? Priority { get; set; }

        public int? AssignedUserId { get; set; }

        // Qualification fields
        public bool IsQualified { get; set; }
        public DateTime? QualifiedOn { get; set; }
        public int? QualifiedById { get; set; }
        public string? QualificationNotes { get; set; }

        // Opportunity related
        public decimal? Budget { get; set; }
        public DateTime? DecisionDate { get; set; }
        public int? Probability { get; set; }

        // Product items entered on lead capture
        public List<LeadProductDto>? ProductItems { get; set; }

        // Attachments metadata (JSON of filenames)
        public string? AttachmentsJson { get; set; }

        // UI-friendly display fields
        public string? StatusName { get; set; }
        public string? AssignedUserName { get; set; }
    }
}
