using System.Collections.Generic;
using seashore_CRM.Common.Constants;

namespace seashore_CRM.Models.DTOs
{
    public class UserListDto
    {
        public int Id { get; set; } 
        public string FullName { get; set; } = null!;
        public string? Contact { get; set; }
        public string Email { get; set; } = null!;
        // kept singular "Role" as this project uses that name in several places
        public List<string> Role { get; set; } = new List<string>();
        public UserStatus Status { get; set; } = UserStatus.Active;
        public string? Region { get; set; }
        public bool IsActive { get; set; }
        public int? ReportToUserId { get; set; }

        // for ReportToName
        public string? ReportToName { get; set; }

        // Added to support login/other controllers that read password and role id from DTO
        public string? PasswordHash { get; set; }
        public int RoleId { get; set; }
    }

    public class UserDetailDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Contact { get; set; }
        public string? Region { get; set; }
        public string? Designation { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public UserStatus Status { get; set; } = UserStatus.Active;
        public bool IsActive { get; set; } = true;
        public int? ReportToUserId { get; set; }
        public string? ReportToName { get; set; }

        // Added to support login/other controllers that read password and role id from DTO
        public string? PasswordHash { get; set; }
        public int RoleId { get; set; }

    }

    public class UserCreateDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? ConfirmPassword { get; set; }
        public string FullName { get; set; } = null!;
        public string? Contact { get; set; }
        public string? Region { get; set; }
        public string? Role { get; set; }

        public int RoleId { get; set; }
        public int? ReportToUserId { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Designation { get; set; }
    }

    public class UserUpdateDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Contact { get; set; }
        public string? Region { get; set; }
        public string? Designation { get; set; }
        public int RoleId { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public int? ReportToUserId { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Active;
        public bool IsActive { get; set; } = true;
        public string? NewPassword { get; set; }
        public string? NewPasswordConfirm { get; set; }
    }
}