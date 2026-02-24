using System.Collections.Generic;
using seashore_CRM.Common.Constants;

namespace Seashore_CRM.ViewModels
{
    public class UserListViewModel
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<string> Roles { get; set; } = new List<string>();
        public UserStatus Status { get; set; } = UserStatus.Active;
        public string? Region { get; set; }
        public bool IsActive { get; set; }
        public string? ReportToName { get; set; }
    }
}