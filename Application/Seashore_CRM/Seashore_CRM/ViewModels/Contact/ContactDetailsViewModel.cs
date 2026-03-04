namespace Seashore_CRM.ViewModels.Contact
{
    internal class ContactDetailsViewModel
    {
        public int Id { get; set; }
        public string? CompanyName { get; set; }
        public string? ContactName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Designation { get; set; }
        public bool IsActive { get; set; }
    }
}