using Microsoft.Extensions.Logging;
using seashore_CRM.BLL.Services.Service_Interfaces;
using seashore_CRM.DAL.Data;
using seashore_CRM.Models.Entities;
using System;
using System.Threading.Tasks;

namespace seashore_CRM.BLL.Services
{
    public class SystemLogService : ISystemLogService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SystemLogService> _logger;

        public SystemLogService(AppDbContext db, ILogger<SystemLogService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task LogAsync(SystemLog log)
        {
            if (log == null) return;

            try
            {
                // ensure timestamp
                if (log.LoggedAt == default) log.LoggedAt = DateTime.UtcNow;

                await _db.SystemLogs.AddAsync(log);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // fallback to console or logger to avoid throwing from logger
                _logger.LogError(ex, "Failed to persist system log. Original message: {Message}", log?.Message);
            }
        }

        public async Task LogExceptionAsync(Exception ex, string source = null, string? correlationId = null)
        {
            if (ex == null) return;
            var log = new SystemLog
            {
                Level = "Error",
                Message = ex.Message,
                Exception = ex.ToString(),
                Source = source,
                CorrelationId = correlationId,
                LoggedAt = DateTime.UtcNow
            };
            await LogAsync(log);
        }

        public async Task LogMessageAsync(string level, string message, string? source = null, string? correlationId = null)
        {
            var log = new SystemLog
            {
                Level = level,
                Message = message,
                Source = source,
                CorrelationId = correlationId,
                LoggedAt = DateTime.UtcNow
            };
            await LogAsync(log);
        }
    }
}