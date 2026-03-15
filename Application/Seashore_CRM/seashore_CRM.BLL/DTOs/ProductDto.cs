using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seashore_CRM.ApplicationLayer.DTOs
{
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Product Name is required")]
        [StringLength(150)]
        public string ProductName { get; set; } = null!;

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        //[Required(ErrorMessage = "Product Group is required")]
        public int? ProductGroupId { get; set; }

        [Required(ErrorMessage = "Cost is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Cost must be positive")]
        public decimal Cost { get; set; }

        //[Required(ErrorMessage = "Tax Percentage is required")]
        [Range(0, 100, ErrorMessage = "Tax must be between 0 and 100")]
        public decimal? TaxPercentage { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ProductUpdateDto
    {
        [Required]
        public int Id { get; set; }  // For updating

        [Required(ErrorMessage = "Product Name is required")]
        [StringLength(150)]
        public string ProductName { get; set; } = null!;

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        //[Required(ErrorMessage = "Product Group is required")]
        public int ProductGroupId { get; set; }

        [Required(ErrorMessage = "Cost is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Cost must be positive")]
        public decimal Cost { get; set; }

        //[Required(ErrorMessage = "Tax Percentage is required")]
        [Range(0, 100, ErrorMessage = "Tax must be between 0 and 100")]
        public decimal? TaxPercentage { get; set; }

        public bool IsActive { get; set; } = true;
    }
    public class ProductListDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string ProductGroupName { get; set; } = null!;
        public decimal Cost { get; set; }
        public decimal? TaxPercentage { get; set; }
        public bool IsActive { get; set; }
        public int? ProductGroupId { get; set; }
        public int CategoryId { get; set; }
    }

    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public int? ProductGroupId { get; set; }
        public string ProductGroupName { get; set; } = null!;
        public decimal Cost { get; set; }
        public decimal? TaxPercentage { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
