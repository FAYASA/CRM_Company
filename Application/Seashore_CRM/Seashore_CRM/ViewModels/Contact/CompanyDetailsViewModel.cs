namespace Seashore_CRM.ViewModels.Contact
{
    public class CompanyDetailsViewModel
    {
        public int CompanyId { get; set; }

        public string CompanyName { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? Industry { get; set; }
        public bool IsActive { get; internal set; }

        // List of contacts belonging to this company
        public IEnumerable<ContactListViewModel>? Contacts { get; set; }

    }
}
