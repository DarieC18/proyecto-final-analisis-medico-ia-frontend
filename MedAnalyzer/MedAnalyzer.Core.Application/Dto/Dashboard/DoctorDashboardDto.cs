namespace MedAnalyzer.Core.Application.Dto.Dashboard
{
    public class DoctorDashboardDto
    {
        public int TotalPatients { get; set; }
        public int ActiveAlerts { get; set; }
        public int TotalAppointmentsToday { get; set; }

        public int CompletedAppointmentsToday { get; set; }

        public int PendingAppointmentsToday { get; set; }

        
        public int TotalAiAnalyses { get; set; }

        public int PendingAiAnalyses { get; set; }

        public int ApprovedAiAnalyses { get; set; }

        public int RejectedAiAnalyses { get; set; }
    }
}


