using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Repositories.Repository_Interfaces;
using seashore_CRM.Models.Entities;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace seashore_CRM.BLL.Services
{
    public class UserActivityService : IUserActivityService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<UserActivityService> _logger;

        public UserActivityService(IUnitOfWork uow, ILogger<UserActivityService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task LogAsync(UserActivity activity)
        {
            if (activity == null) return;
            try
            {
                if (activity.PerformedAt == default) activity.PerformedAt = System.DateTime.UtcNow;
                await _uow.UserActivities.AddAsync(activity);
                await _uow.CommitAsync();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Failed to persist user activity");
            }
        }

        public Task LogLoginAsync(string userId, string userName, string? correlationId = null)
        {
            var a = new UserActivity
            {
                UserId = userId,
                UserName = userName,
                Action = "Login",
                PerformedAt = System.DateTime.UtcNow,
                CorrelationId = correlationId
            };
            return LogAsync(a);
        }

        public Task LogLogoutAsync(string userId, string userName, string? correlationId = null)
        {
            var a = new UserActivity
            {
                UserId = userId,
                UserName = userName,
                Action = "Logout",
                PerformedAt = System.DateTime.UtcNow,
                CorrelationId = correlationId
            };
            return LogAsync(a);
        }

        public Task LogEntityActionAsync(string userId, string userName, string action, string entityName,
            string? entityId = null, string? details = null, string? correlationId = null)
        {
            var a = new UserActivity
            {
                UserId = userId,
                UserName = userName,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Details = details,
                PerformedAt = System.DateTime.UtcNow,
                CorrelationId = correlationId
            };
            return LogAsync(a);
        }

        public async Task<IEnumerable<UserActivity>> GetRecentAsync(int count = 50)
        {
            return (await _uow.UserActivities.GetAllAsync()).OrderByDescending(u => u.PerformedAt).Take(count).AsEnumerable();
        }

        public async Task<IEnumerable<UserActivity>> QueryAsync(string? userFilter = null, string? actionFilter = null, string? entityFilter = null, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 50)
        {
            var q = (await _uow.UserActivities.GetAllAsync()).AsQueryable();

            if (!string.IsNullOrWhiteSpace(userFilter))
            {
                q = q.Where(u => u.UserName != null && u.UserName.Contains(userFilter));
            }
            if (!string.IsNullOrWhiteSpace(actionFilter))
            {
                q = q.Where(u => u.Action != null && u.Action == actionFilter);
            }
            if (!string.IsNullOrWhiteSpace(entityFilter))
            {
                q = q.Where(u => u.EntityName != null && u.EntityName.Contains(entityFilter));
            }
            if (from.HasValue)
            {
                q = q.Where(u => u.PerformedAt >= from.Value);
            }
            if (to.HasValue)
            {
                q = q.Where(u => u.PerformedAt <= to.Value);
            }

            var skip = (Math.Max(1, page) - 1) * Math.Max(1, pageSize);
            return q.OrderByDescending(u => u.PerformedAt).Skip(skip).Take(pageSize).ToList();
        }
    }
}