namespace seashore_CRM.Models.Entities
{
    public class Company : BaseEntity
    {
        public string CompanyName { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        //public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        // New fields
        public string? Pin { get; set; }
        public string? Website { get; set; }
        public string? AddressPost { get; set; }
        public string? Industry {get; set; }    
    }
}
