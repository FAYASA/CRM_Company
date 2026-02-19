using System;

namespace seashore_CRM.Models.Entities
{
    public class Comment : BaseEntity
    {
        public int? LeadId { get; set; }
        public int? CustomerId { get; set; }
        public string CommentText { get; set; } = null!;
        public int? CreatedById { get; set; }

        public Lead? Lead { get; set; }
        public Company? Customer { get; set; }
        public User? CreatedBy { get; set; }
    }
}
