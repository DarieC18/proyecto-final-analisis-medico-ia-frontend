namespace MedAnalyzer.Core.Application.Dto.AiAnalisys
{
    public class GenerateAiAnalysisRequestDto
    {
        public int AppointmentId { get; set; }
        public required string AnalysisType { get; set; }
    }
}
