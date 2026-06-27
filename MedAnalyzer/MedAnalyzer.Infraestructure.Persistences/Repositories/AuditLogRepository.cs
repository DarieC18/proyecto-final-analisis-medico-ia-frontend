using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Interfaces;
using MedAnalyzer.Infraestructure.Persistences.Context;
using Microsoft.EntityFrameworkCore;

namespace MedAnalyzer.Infraestructure.Persistences.Repositories
{
    public class AuditLogRepository : BaseRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(MedAnalyzerContextDb context) : base(context) { }

        public async Task<List<AuditLog>> GetFilteredAsync(string? userId, string? action, DateTime? from, DateTime? to)
        {
            var query = GetAllQuery();

            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(a => a.UserId == userId);

            if (!string.IsNullOrWhiteSpace(action))
                query = query.Where(a => a.Action == action);

            if (from.HasValue)
                query = query.Where(a => a.CreatedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(a => a.CreatedAt <= to.Value);

            return await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
        }
    }
}