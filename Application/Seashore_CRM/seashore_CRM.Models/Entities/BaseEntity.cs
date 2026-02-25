using System;
using System.ComponentModel.DataAnnotations;

namespace seashore_CRM.Models.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; } = null;

        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }

        public bool IsActive { get; set; } = true;

        // Concurrency token (prevents overwrite issues)
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
