using MedAnalyzer.Core.Domain.Base;

namespace MedAnalyzer.Core.Domain.Entities
{
    public class AiAnalysis : BaseEntity<int>
    {
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }

        public int? DocumentId { get; set; }
        public int RequestedByUserId { get; set; }
        public required string AnalysisType { get; set; }
        public required string  PromptUsed { get; set; }
        public required string AiResponse { get; set; }
        public required string ModelUsed { get; set; }

        public Patient? Patient { get; set; }
        public Appointment? Appointment { get; set; }
        public MedicalDocument? Document { get; set; }
        public ICollection<Recommendation>? AiAnalysisResults { get; set; }

    }
}
