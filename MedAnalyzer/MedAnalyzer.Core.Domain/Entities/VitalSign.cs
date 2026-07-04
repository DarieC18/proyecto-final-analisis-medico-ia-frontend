using MedAnalyzer.Core.Domain.Base;

namespace MedAnalyzer.Core.Domain.Entities
{
    public class VitalSign : BaseEntity<int>
    {
        public int AppointmentId { get; set; }
        public decimal Temperature { get; set; }
        public int HeartRate { get; set; }
        public int SystolicPressure { get; set; }
        public int DiastolicPressure { get; set; }
        public decimal OxygenSaturation { get; set; }
        public decimal? Glucose { get; set; }
        public DateTime MeasuredAt { get; set; }

        public Appointment? Appointment { get; set; }
    }
}