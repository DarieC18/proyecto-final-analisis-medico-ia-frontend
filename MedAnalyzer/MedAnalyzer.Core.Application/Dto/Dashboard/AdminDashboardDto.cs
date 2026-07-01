using MedAnalyzer.Core.Application.Dto.AuditLog;

namespace MedAnalyzer.Core.Application.Dto.Dashboard
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int TotalActiveUsers { get; set; }
        public int TotalInactiveUsers { get; set; }
        public int TotalPatients { get; set; }
        public Dictionary<string, int> AppointmentsByStatus { get; set; } = [];
        public int TotalAiAnalyses { get; set; }
        public int ActiveAlerts { get; set; }
        public List<AuditLogDto> LatestAuditLogs { get; set; } = [];
        public int TotalAppointmentsToday { get; set; }
        public int CompletedAppointmentsToday { get; set; }
        public int PendingAppointmentsToday { get; set; }
        public int PendingAiAnalyses { get; set; }
        public int ApprovedAiAnalyses { get; set; }
        public int RejectedAiAnalyses { get; set; }
    }
}
