using seashore_CRM.DomainModelLayer.Entities;
using System.ComponentModel.DataAnnotations;

namespace seashore_CRM.Models.Entities
{
    public class Category : BaseEntity
    {
        [Required]
        [MaxLength(150)]
        public string CategoryName { get; set; } = null!;

        public ICollection<ProductGroup>? ProductGroups { get; set; }
    }
}
