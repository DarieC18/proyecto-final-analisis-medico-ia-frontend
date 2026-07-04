namespace MedAnalyzer.Core.Application.Dto.Symptom
{
    public class SymptomDto
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public required string Name { get; set; }
        public required string Severity { get; set; }
        public DateOnly StartedAt { get; set; }
        public string? Notes { get; set; }
    }
}
