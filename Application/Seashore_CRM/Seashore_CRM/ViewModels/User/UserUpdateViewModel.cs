using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Seashore_CRM.ViewModels.User
{
    public class UserUpdateViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        [MaxLength(150)]
        [Remote("VerifyFullName", "Users", AdditionalFields = nameof(Id), HttpMethod = "Post", ErrorMessage = "Full name is already in use.")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Contact")]
        [MaxLength(50)]
        [Remote("VerifyContact", "Users", AdditionalFields = nameof(Id), HttpMethod = "Post", ErrorMessage = "Contact is already in use.")]
        public string? Contact { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [MaxLength(150)]
        [Remote("VerifyEmail", "Users", AdditionalFields = nameof(Id), HttpMethod = "Post", ErrorMessage = "Email is already in use.")]
        public string Email { get; set; } = null!;

        [Display(Name = "Designation")]
        public string? Designation { get; set; }

        [Display(Name = "Region")]
        public string? Region { get; set; }

        [Display(Name = "Report To User")]
        public int? ReportToUserId { get; set; }

        [Required]
        [Display(Name = "Role")]
        public int RoleId { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password and Confirm Password do not match.")]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }

        // Select lists for the view
        public IEnumerable<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Users { get; set; } = new List<SelectListItem>();
    }
}
