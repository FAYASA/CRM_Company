using System;
using System.Collections.Generic;

namespace seashore_CRM.Models.Entities
{
    public class Sale : BaseEntity
    {
        public int OpportunityId { get; set; }
        public int CustomerId { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.UtcNow;

        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrossTotal { get; set; }

        public int CreatedById { get; set; }

        public Opportunity? Opportunity { get; set; }
        public Company? Customer { get; set; }
        public List<SaleItem>? Items { get; set; }
    }
}
