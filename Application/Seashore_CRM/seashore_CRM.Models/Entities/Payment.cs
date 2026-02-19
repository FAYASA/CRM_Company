using System;

namespace seashore_CRM.Models.Entities
{
    public class Payment : BaseEntity
    {
        public int InvoiceId { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string? PaymentMethod { get; set; }
        public int? ReceivedBy { get; set; }

        public Invoice? Invoice { get; set; }
        public User? Receiver { get; set; }
    }
}
