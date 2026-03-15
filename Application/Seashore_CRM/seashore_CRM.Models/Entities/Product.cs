using seashore_CRM.DomainModelLayer.Entities;
using System.ComponentModel.DataAnnotations;

namespace seashore_CRM.Models.Entities
{
    public class Product : BaseEntity
    {
        [Required]
        public string ProductName { get; set; } = null!;

        public int CategoryId { get; set; }

        public int? ProductGroupId { get; set; }

        public decimal Cost { get; set; }

        public decimal? TaxPercentage { get; set; }

        public Category? Category { get; set; }

        public ProductGroup? ProductGroup { get; set; }
    }
}
