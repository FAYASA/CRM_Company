using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using seashore_CRM.ApplicationLayer.DTOs;

namespace seashore_CRM.BLL.DTOs
{
    public class LeadDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Lead Type is required")]
        public string LeadType { get; set; } = null!;

        public int? CompanyId { get; set; }
        public int? ContactId { get; set; }
        // For individual leads, reference the IndividualCustomer entity
        public int? IndividualCustomerId { get; set; }

        public int? SourceId { get; set; }
        public int? StatusId { get; set; }

        // for ActivityType
        public string? ActivityType { get; set; }

        // for FollowUpDate
        public DateTime? FollowUpDate { get; set; }

        // for FollowUpTime
        public TimeSpan? FollowUpTime { get; set; }

        // for ExpectedClosureDate
        public DateTime? ExpectedClosureDate { get; set; }
        public int? ActivityId { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public string? Priority { get; set; }

        public int? AssignedUserId { get; set; }

        // Qualification fields
        public bool IsQualified { get; set; }
        public DateTime? QualifiedOn { get; set; }
        public int? QualifiedById { get; set; }
        public string? QualificationNotes { get; set; }

        // Mark if lead already converted
        public bool IsConverted { get; set; }

        // Opportunity related
        public decimal? Budget { get; set; }
        public DateTime? DecisionDate { get; set; }
        public int? Probability { get; set; }

        // Product items entered on lead capture
        public List<LeadProductDto>? ProductItems { get; set; }

        // Comments entered during lead capture
        public List<CommentDto>? Comments { get; set; }

        // User Rights for the leads
        public List<UserLeadRightDto>? UserLeadRights { get; set; }

        // Attachments metadata (JSON of filenames)
        public string? AttachmentsJson { get; set; }

        // UI-friendly display fields
        public string? StatusName { get; set; }
        public string? AssignedUserName { get; set; }

        // Additional UI fields populated from DB
        public decimal GrossTotal { get; set; }
        public int Units { get; set; }
        public string? LatestActivity { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ClosureDate { get; set; }
        public List<string>? ProductNames { get; set; }

        // New: display customer name (company or individual contact)
        public string? CustomerName { get; set; }
        // New: display customer location (company city or contact mobile/phone)
        public string? CustomerLocation { get; set; }

        // Status -> activities mapping for the lead's status
        public List<string>? StatusActivities { get; set; }

        public int? CategoryId { get; set; }

        // New aggregate fields captured from client (model binding)
        public decimal? SubTotal { get; set; }
        public decimal? TaxTotal { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal? GrossProfit { get; set; }

        // Selected activities from client-side (optional)
        public List<string>? SelectedActivities { get; set; }

        // RowVersion for optimistic concurrency
        public byte[]? RowVersion { get; set; }
    }
}
