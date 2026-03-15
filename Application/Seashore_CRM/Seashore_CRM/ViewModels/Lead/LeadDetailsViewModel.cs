using seashore_CRM.BLL.DTOs;
using System.Collections.Generic;

namespace Seashore_CRM.ViewModels.Lead
{
    public class LeadDetailsViewModel
    {
        public LeadDto Lead { get; set; } = new LeadDto();

        public IEnumerable<ActivityViewModel>? Activities { get; set; }
        public IEnumerable<CommentViewModel>? Comments { get; set; }

        public class ActivityViewModel
        {
            public int Id { get; set; }
            public string? ActivityType { get; set; }
            public string? Description { get; set; }
            public System.DateTime ActivityDate { get; set; }
        }

        public class CommentViewModel
        {
            public int Id { get; set; }
            public string? CommentText { get; set; }
            public System.DateTime CreatedDate { get; set; }
        }
    }
}
