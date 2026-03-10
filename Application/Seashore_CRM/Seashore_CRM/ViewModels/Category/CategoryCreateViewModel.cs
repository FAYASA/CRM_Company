using System.ComponentModel.DataAnnotations;

namespace Seashore_CRM.ViewModels.Category
{
    public class CategoryCreateViewModel
    {
        [Required]
        [Display(Name = "Category Name")]
        [StringLength(150, ErrorMessage = "Category name must be at most 150 characters")]
        public string CategoryName { get; set; } = null!;
    }
}
