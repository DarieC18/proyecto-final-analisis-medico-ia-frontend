using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Domain.Interfaces
{
    public interface IAuditLogRepository : IBaseRepository<AuditLog>
    {
        Task<List<AuditLog>> GetFilteredAsync(string? userId, string? action, DateTime? from, DateTime? to);
    }

}