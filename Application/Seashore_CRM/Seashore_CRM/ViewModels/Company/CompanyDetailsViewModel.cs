namespace Seashore_CRM.ViewModels.Company
{
    public class CompanyDetailsViewModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? City { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }   
        public string? Industry { get; set; }
        public bool IsActive { get; set; }
        public string? Address { get; set; }
        public string? AddressPost { get; set; }
        public string? Pin { get; set; }   
    }
}
