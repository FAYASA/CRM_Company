using System.ComponentModel.DataAnnotations;

namespace Seashore_CRM.ViewModels.Category
{
    public class CategoryListViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category Name")]

        public string CategoryName { get; set; } = null!;

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

    }
}
