namespace seashore_CRM.Models.Entities
{
    public class Company : BaseEntity
    {
        public string CompanyName { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
