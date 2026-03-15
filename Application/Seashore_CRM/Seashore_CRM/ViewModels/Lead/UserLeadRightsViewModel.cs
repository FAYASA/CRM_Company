using System.ComponentModel.DataAnnotations;

namespace Seashore_CRM.ViewModels.Lead
{
    public class UserLeadRightsViewModel
    {
        // database id of the rights record (optional for newly added rows)
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        // LeadId is optional on the client when creating a new Lead (server assigns actual LeadId)
        public int LeadId { get; set; }

        public string UserName { get; set; } = string.Empty;
        public string LeadName { get; set; } = string.Empty;

        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
    }
}
