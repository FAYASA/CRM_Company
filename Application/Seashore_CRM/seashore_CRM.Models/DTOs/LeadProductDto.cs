namespace seashore_CRM.Models.DTOs
{
    public class LeadProductDto
    {
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxPercentage { get; set; }
    }
}