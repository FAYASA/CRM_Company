using System.Threading.Tasks;
using seashore_CRM.Models.Entities;
using System.Collections.Generic;
using System;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface IUserActivityService
    {
        Task LogAsync(UserActivity activity);
        Task LogLoginAsync(string userId, string userName, string? correlationId = null);
        Task LogLogoutAsync(string userId, string userName, string? correlationId = null);
        Task LogEntityActionAsync(string userId, string userName, string action, string entityName, string? entityId = null, string? details = null, string? correlationId = null);
        Task<IEnumerable<UserActivity>> GetRecentAsync(int count = 50);

        // Query with filters and paging for admin UI
        Task<IEnumerable<UserActivity>> QueryAsync(string? userFilter = null, string? actionFilter = null, string? entityFilter = null, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 50);
    }
}