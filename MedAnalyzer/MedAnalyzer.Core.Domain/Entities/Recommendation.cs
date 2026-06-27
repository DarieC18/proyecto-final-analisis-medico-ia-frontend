using MedAnalyzer.Core.Domain.Base;

namespace MedAnalyzer.Core.Domain.Entities
{
    public class Recommendation : BaseEntity<int>
    {
        public int AiAnalysisId { get; set; }
        public int AppointmentId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string RiskLevel { get; set; }

        public AiAnalysis? AiAnalysis { get; set; }
        public Appointment? Appointment { get; set; }

    }
}
