using System;

namespace seashore_CRM.Models.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string PasswordHash { get; set; } = null!;
        public int RoleId { get; set; }
        public bool IsActive { get; set; } = true;

        public Role? Role { get; set; }
    }
}
