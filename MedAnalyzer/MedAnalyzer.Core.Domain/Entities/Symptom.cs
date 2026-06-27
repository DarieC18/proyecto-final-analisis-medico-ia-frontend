namespace MedAnalyzer.Core.Domain.Entities
{
    public class Symptom 
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public required string Name { get; set; }
        public required string Severity { get; set; }
        public DateOnly StartedAt { get; set; }
        public string? Notes { get; set; }

        public Appointment? Appointment { get; set; }
      
    }
}
