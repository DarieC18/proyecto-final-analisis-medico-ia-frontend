using MedAnalyzer.Core.Domain.Base;

namespace MedAnalyzer.Core.Domain.Entities
{
    public class MedicalRecord : BaseEntity<int>
    {
        public int PatientId { get; set; }
        public int AppointmentId {get; set; }
        public int CreatedByUserId { get; set; }
        public required string DiagnosisInitial { get; set; }
        public required string Notes { get; set; }

        public Patient? Patient { get; set; }
        public Appointment? Appointment { get; set; }
    }
}
