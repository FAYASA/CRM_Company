namespace seashore_CRM.Models.Entities
{
    public class Contact : BaseEntity
    {
        public int? CompanyId { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Position { get; set; }

        public Company? Company { get; set; }
    }
}
