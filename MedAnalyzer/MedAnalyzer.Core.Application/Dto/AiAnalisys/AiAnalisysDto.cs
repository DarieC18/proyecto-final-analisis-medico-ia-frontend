using MedAnalyzer.Core.Application.Base;

namespace MedAnalyzer.Core.Application.Dto.AiAnalisys
{
    public class AiAnalisysDto : BaseDto<int>
    {
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }
        public int? DocumentId { get; set; }
        public required string RequestedByUserId { get; set; }
        public required string AnalysisType { get; set; }
        public required string PromptUsed { get; set; }
        public required string AiResponse { get; set; }
        public required string ModelUsed { get; set; }
        public required string Status { get; set; }
        public bool IsReviewed { get; set; }

    }
}
