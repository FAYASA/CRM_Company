using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Seashore_CRM.ViewModels.ProductGroup
{
    public class ProGroupUpdateViewModel
    {
        public int Id { get; set; }
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
