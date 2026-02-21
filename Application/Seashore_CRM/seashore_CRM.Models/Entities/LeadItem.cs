using System;

namespace seashore_CRM.Models.Entities
{
    public class LeadItem : BaseEntity
    {
        public int LeadId { get; set; }
        public int ProductId { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal LineTotal { get; set; }

        public Lead? Lead { get; set; }
        public Product? Product { get; set; }
    }
}
