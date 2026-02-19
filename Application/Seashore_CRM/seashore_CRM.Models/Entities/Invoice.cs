using System;

namespace seashore_CRM.Models.Entities
{
    public class Invoice : BaseEntity
    {
        public int SaleId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }

        public Sale? Sale { get; set; }
    }
}
