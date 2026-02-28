using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Seashore_CRM.ViewModels.Company
{
    public class CompanyUpdateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters.")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } = null!;

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "City")]
        public string? City { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Pin Code")]
        public string? Pin { get; set; }

        [Url(ErrorMessage = "Invalid website URL.")]
        [Display(Name = "Website")]
        public string? Website { get; set; }

        [Display(Name = "Postal Address")]
        public string? AddressPost { get; set; }

        [Display(Name = "Industry")]
        public string? Industry { get; set; }

        // For Isactive toggle
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public byte[]? RowVersion { get; set; }

        public IEnumerable<SelectListItem>? IndustryList { get; set; }
    }
}
