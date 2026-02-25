using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Seashore_CRM.ViewModels.User
{
    public class UserCreateViewModel
    {
        [Required]
        [MaxLength(150)]
        [Remote("VerifyFullName", "Users", HttpMethod = "Post", ErrorMessage = "Full name is already in use.")]
        public string FullName { get; set; }

        [MaxLength(50)]
        [Remote("VerifyContact", "Users", HttpMethod = "Post", ErrorMessage = "Contact is already in use.")]
        public string Contact { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        [Remote("VerifyEmail", "Users", HttpMethod = "Post", ErrorMessage = "Email is already in use.")]
        public string Email { get; set; }

        public string Designation { get; set; }
        public string Region { get; set; }
        public string? ReportToUserId { get; set; }
        public string? RoleId { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
        public bool IsActive { get; set; } = true;

        public IEnumerable<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Users { get; set; } = new List<SelectListItem>();
    }
}
