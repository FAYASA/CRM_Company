using System.Collections.Generic;
using seashore_CRM.Common.Constants;

namespace Seashore_CRM.ViewModels.User
{
public class UserListViewModel
{
    public int Id { get; set; } 
    public string? UserName { get; set; } 
    public string Email { get; set; } = null!;
    public List<string> Roles { get; set; } = new List<string>();
    public string? Region { get; set; }
    public bool IsActive { get; set; }
    public string? ReportToName { get; set; }
}
}