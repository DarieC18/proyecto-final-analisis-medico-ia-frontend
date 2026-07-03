using MedAnalyzer.Core.Application.Dto.AuditLog;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IAuditLogService : IBaseServices<AuditLog, AuditLogDto>
    {
        Task<List<AuditLogDto>> GetFilteredAsync(string? userId, string? action, DateTime? from, DateTime? to);

        Task LogAsync(string userId, string action, string entityName, string entityId);
    }
}