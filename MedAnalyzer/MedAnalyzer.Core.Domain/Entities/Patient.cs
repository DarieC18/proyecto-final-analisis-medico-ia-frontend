using MedAnalyzer.Core.Domain.Base;

namespace MedAnalyzer.Core.Domain.Entities
{
    public class Patient : BaseEntity<int>
    {
        public required string UserId { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string IdentificationType { get; set; } = string.Empty;
        public string PatientType { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public ICollection<Appointment>? Appointments { get; set; }
        public ICollection<MedicalRecord>? MedicalRecords { get; set; }
        public ICollection<MedicalDocument>? MedicalDocuments { get; set; }
        public ICollection<VitalSign>? VitalSigns { get; set; }
        public ICollection<AiAnalysis>? AiAnalyses { get; set; }
        public ICollection<Alert>? Alerts { get; set; }
    }
}
