using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using seashore_CRM.DAL.Data;
using seashore_CRM.Models.Entities;
using seashore_CRM.BLL.Services.Service_Interfaces;

namespace Seashore_CRM.Middleware
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, AppDbContext db, ISystemLogService logService)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var correlation = context.TraceIdentifier;
                try
                {
                    await logService.LogExceptionAsync(ex, context.Request.Path, correlation);
                }
                catch
                {
                    // fallback: try write directly to DB to ensure log is persisted
                    try
                    {
                        var log = new SystemLog
                        {
                            Level = "Error",
                            Message = ex.Message,
                            Exception = ex.ToString(),
                            Source = context.Request.Path,
                            CorrelationId = correlation,
                            LoggedAt = DateTime.UtcNow
                        };
                        db.SystemLogs.Add(log);
                        await db.SaveChangesAsync();
                    }
                    catch { }
                }

                // rethrow after logging
                throw;
            }
        }
    }
}
