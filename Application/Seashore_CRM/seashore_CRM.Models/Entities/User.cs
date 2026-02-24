using System;
using seashore_CRM.Common.Constants;

namespace seashore_CRM.Models.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Contact { get; set; }
        public string? Region { get; set; }
        public string PasswordHash { get; set; } = null!;
        public int RoleId { get; set; }
        public bool IsActive { get; set; } = true;

        // New fields
        public UserStatus Status { get; set; } = UserStatus.Active;

        // Reporting user (manager/leader)
        public int? ReportToUserId { get; set; }
        public User? ReportToUser { get; set; }

        public Role? Role { get; set; }
    }
}
