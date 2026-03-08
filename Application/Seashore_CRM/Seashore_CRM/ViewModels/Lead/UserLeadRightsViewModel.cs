namespace Seashore_CRM.ViewModels.Lead
{
    public class UserLeadRightsViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string LeadName { get; set; }
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
    }
}
