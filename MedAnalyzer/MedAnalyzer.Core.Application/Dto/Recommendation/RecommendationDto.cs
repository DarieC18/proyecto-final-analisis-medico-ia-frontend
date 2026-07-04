using MedAnalyzer.Core.Application.Base;

namespace MedAnalyzer.Core.Application.Dto.Recommendation
{
    public class RecommendationDto : BaseDto<int>
    {
        public int AiAnalysisId { get; set; }
        public int AppointmentId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string RiskLevel { get; set; }
    }
}
