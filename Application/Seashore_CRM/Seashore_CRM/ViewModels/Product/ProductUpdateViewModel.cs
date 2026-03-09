using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Seashore_CRM.ViewModels.Product
{
    public class ProductUpdateViewModel
    {
        public int Id { get; set; }

        [Required]
        public string ProductName { get; set; } = null!;

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Pro.Group")]
        public int ProductGroupId { get; set; }

        public decimal Cost { get; set; }

        [Display(Name = "Tax %")]
        public decimal TaxPercentage { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }

        public IEnumerable<SelectListItem>? ProductGroups { get; set; }
    }
}
