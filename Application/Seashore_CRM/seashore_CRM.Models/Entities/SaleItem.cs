namespace seashore_CRM.Models.Entities
{
    public class SaleItem : BaseEntity
    {
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal LineTotal { get; set; }

        public Sale? Sale { get; set; }
        public Product? Product { get; set; }
    }
}
