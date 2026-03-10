using System.ComponentModel.DataAnnotations;

namespace Seashore_CRM.ViewModels.Category
{
    public class CategoryUpdateViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        [StringLength(150, ErrorMessage = "Category name must be at most 150 characters")]
        public string CategoryName { get; set; } = null!;

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public byte[]? RowVersion { get; set; }
    }
}
