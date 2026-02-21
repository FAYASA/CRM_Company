namespace seashore_CRM.Models.DTOs
{
    public class LeadProductDto
    {
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxPercentage { get; set; }

        // new fields to capture computed and additional metadata
        public int? CategoryId { get; set; }
        public string? ProductGroup { get; set; }
        public decimal SaleValue { get; set; }
        public decimal TaxValue { get; set; }
        public decimal GrossTotal { get; set; }
        public decimal Cost { get; set; }
        public decimal GrossProfit { get; set; }
    }
}