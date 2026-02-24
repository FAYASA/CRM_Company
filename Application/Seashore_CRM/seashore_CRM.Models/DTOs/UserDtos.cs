using System.Collections.Generic;
using seashore_CRM.Common.Constants;

namespace seashore_CRM.Models.DTOs
{
    public class UserListDto
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<string> Roles { get; set; } = new List<string>();
        public UserStatus Status { get; set; } = UserStatus.Active;
        public string? Region { get; set; }
        public bool IsActive { get; set; }
        public string? ReportToUserId { get; set; }
    }

    public class UserDetailDto
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Contact { get; set; }
        public string? Region { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public UserStatus Status { get; set; } = UserStatus.Active;
        public bool IsActive { get; set; } = true;
        public string? ReportToUserId { get; set; }
    }

    public class UserCreateDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Contact { get; set; }
        public string? Region { get; set; }
        public string? Role { get; set; }
        public string? ReportToUserId { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Active;
        public bool IsActive { get; set; } = true;
    }

    public class UserUpdateDto
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Contact { get; set; }
        public string? Region { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string? ReportToUserId { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Active;
        public bool IsActive { get; set; } = true;
        public string? NewPassword { get; set; }
    }
}