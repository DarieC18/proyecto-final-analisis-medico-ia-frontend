using MedAnalyzer.Core.Domain.Base;

namespace MedAnalyzer.Core.Domain.Entities
{
    public class VitalSign : BaseEntity<int>
    {
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }
        public required decimal Temperature { get; set; }
        public int HeartRate { get; set; }
        public int DiastolicPressure { get; set; }
        public int SystolicPressure { get; set; }
        public required decimal OxygenSaturation { get; set; }
        public decimal? Glucose { get; set; }
        public DateTime MeasuredAt { get; set; }

        public Appointment? Appointment { get; set; }
        public Patient? Patient { get; set; }
    }
}
