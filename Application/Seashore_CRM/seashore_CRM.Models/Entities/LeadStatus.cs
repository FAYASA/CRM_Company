namespace seashore_CRM.Models.Entities
{
    public class LeadStatus : BaseEntity
    {
        public string StatusName { get; set; } = null!;

        // Activities associated with this status (stored in DB)
        public ICollection<LeadStatusActivity> Activities { get; set; } = new List<LeadStatusActivity>();
    }
}
