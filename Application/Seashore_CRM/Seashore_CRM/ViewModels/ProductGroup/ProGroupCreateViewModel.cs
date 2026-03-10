using Microsoft.AspNetCore.Mvc.Rendering;
using seashore_CRM.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Seashore_CRM.ViewModels.ProductGroup
{
    public class ProGroupCreateViewModel
    {
        [Required]
        [Display(Name = "Group Name")]
        public string GroupName { get; set; } = null!;

        [Required]
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        // Dropdown lists
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

    }
}
