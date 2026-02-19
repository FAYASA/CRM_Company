namespace seashore_CRM.Models.Entities
{
    public class Product : BaseEntity
    {
        public string ProductName { get; set; } = null!;
        public int? CategoryId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal TaxPercentage { get; set; }
        public bool IsActive { get; set; } = true;

        public Category? Category { get; set; }
    }
}
