using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Seashore_CRM.ViewModels.Contact
{
    public class ContactCreateViewModel
    {
        [Required(ErrorMessage = "Contact name is required.")]
        [StringLength(150, ErrorMessage = "Contact name cannot exceed 150 characters.")]
        [Display(Name = "Contact Name")]
        public string ContactName { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [Phone(ErrorMessage = "Invalid mobile number.")]
        [Display(Name = "Mobile")]
        public string? Mobile { get; set; }

        [Display(Name = "Designation")]
        public string? Designation { get; set; }

        [Display(Name = "Company")]
        public int? CompanyId { get; set; }

        // For dropdown selection
        public IEnumerable<SelectListItem>? CompanyList { get; set; }
    }

}
