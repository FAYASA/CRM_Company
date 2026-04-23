using System.Threading.Tasks;
using seashore_CRM.Models.Entities;
using System;

namespace seashore_CRM.BLL.Services.Service_Interfaces
{
    public interface ISystemLogService
    {
        Task LogAsync(SystemLog log);
        Task LogExceptionAsync(Exception ex, string source = null, string? correlationId = null);
        Task LogMessageAsync(string level, string message, string? source = null, string? correlationId = null);
    }
}