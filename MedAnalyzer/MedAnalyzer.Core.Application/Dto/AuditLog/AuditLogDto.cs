namespace MedAnalyzer.Core.Application.Dto.AuditLog
{
    public class AuditLogDto
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public required string UserRole { get; set; }
        public required string Action { get; set; }
        public required string EntityName { get; set; }
        public required string EntityId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}