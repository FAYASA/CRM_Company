using System;

namespace seashore_CRM.Models.Entities
{
    public class SystemLog
    {
        public int Id { get; set; }
        public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
        public string Level { get; set; } = "Error"; // Error, Warning, Info
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public string? Source { get; set; }
        public string? CorrelationId { get; set; }
    }
}