using MedAnalyzer.Core.Domain.Base;

namespace MedAnalyzer.Core.Domain.Entities
{
    public class Patient : BaseEntity<int>
    {
        public required string FullName { get; set; }
        public DateOnly BirthDate { get; set; }
        public required string Gender { get; set; }
        public required string PhoneNumber { get; set; }
        public required string IdentificationNumber { get; set; }
        public required string IdentificationType { get; set; }
        public required string PatientType { get; set; }
        public bool IsActive { get; set; } = true;


        public ICollection<Appointment>? Appointments { get; set; }
        public ICollection<MedicalRecord>? MedicalRecords { get; set; }
        public ICollection<MedicalDocument>? MedicalDocuments { get; set; }
        public ICollection<VitalSign>? VitalSigns { get; set; }
        public ICollection<AiAnalysis>? AiAnalyses { get; set; }
        public ICollection<Alert>? Alerts { get; set; }
     


    }
}
