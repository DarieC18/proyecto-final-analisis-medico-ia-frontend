using MedAnalyzer.Core.Domain.Base;

namespace MedAnalyzer.Core.Domain.Entities
{
    public class AuditLog : BaseEntity<int>
    {
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public required string UserRole { get; set; }
        public required string Action { get; set; }
        public required string EntityName { get; set; }
        public required string EntityId { get; set; }
    }
}