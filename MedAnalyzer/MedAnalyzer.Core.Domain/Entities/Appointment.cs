using MedAnalyzer.Core.Domain.Base;


namespace MedAnalyzer.Core.Domain.Entities
{
    public class Appointment : BaseEntity<int>
    {
        public int PatientId { get; set; }
        public required string DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public required string Status { get; set; } 
        public required string Reason { get; set; } 
        public string? Notes { get; set; } 

        public Patient? Patient { get; set; } 
        public ICollection<MedicalRecord>? MedicalRecords { get; set; }
        public ICollection<MedicalDocument>? MedicalDocuments { get; set; }
        public ICollection<Symptom>? Symptoms { get; set; }
        public ICollection<VitalSign>? VitalSigns { get; set; }
        public ICollection<AiAnalysis>? AiAnalyses { get; set; }
        public ICollection<Recommendation>? Recommendations { get; set; }
        public ICollection<Alert>? Alerts { get; set; }

    }
}
