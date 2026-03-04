namespace Seashore_CRM.ViewModels.Contact
{
    public class ContactListViewModel
    {
        public int ContactId { get; set; }
        public string ContactName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Designation { get; set; }

        public string? CompanyName { get; set; }
        public int Id { get; internal set; }
        public bool IsActive { get; internal set; }
        public int? CompanyId { get; internal set; }
    }
}
