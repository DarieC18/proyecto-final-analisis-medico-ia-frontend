using MedAnalyzer.Core.Domain.Base;


namespace MedAnalyzer.Core.Domain.Entities
{
    public class Alert : BaseEntity<int>
    {
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Severity { get; set; }
        public bool IsResolved { get; set; }

        public Patient? Patient { get; set; }
        public Appointment? Appointment { get; set; }

    }
}
