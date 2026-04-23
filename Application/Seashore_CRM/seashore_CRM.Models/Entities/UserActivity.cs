using System;

namespace seashore_CRM.Models.Entities
{
    public class UserActivity
    {
        public int Id { get; set; }

        // user identity (string to support Identity user ids and custom numeric ids as string)
        public string? UserId { get; set; }
        public string? UserName { get; set; }

        // action performed by user: Login, Logout, Create, Update, Delete, etc.
        public string Action { get; set; } = string.Empty;

        // target entity and identifier (e.g., Lead, Company, Contact)
        public string? EntityName { get; set; }
        public string? EntityId { get; set; }

        // optional details / payload
        public string? Details { get; set; }

        // timestamp and correlation id
        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
        public string? CorrelationId { get; set; }
    }
}