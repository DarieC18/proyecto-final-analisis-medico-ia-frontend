using MedAnalyzer.Core.Application.Dto.Appointment;
using MedAnalyzer.Core.Application.Dto.MedicalDocument;
using MedAnalyzer.Core.Application.Dto.MedicalRecord;

namespace MedAnalyzer.Core.Application.Dto.Patient
{
    public class PatientDetailDto
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public DateOnly BirthDate { get; set; }
        public required string Gender { get; set; }
        public required string PhoneNumber { get; set; }
        public required string IdentificationNumber { get; set; }
        public required string IdentificationType { get; set; }
        public required string PatientType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<AppointmentSummaryDto> Appointments { get; set; } = [];
        public List<MedicalRecordSummaryDto> MedicalRecords { get; set; } = [];
        public List<MedicalDocumentDto> Documents { get; set; } = [];
    }
}
