using MedAnalyzer.Core.Application.Dto.MedicalDocument;
using MedAnalyzer.Core.Application.Dto.MedicalRecord;

namespace MedAnalyzer.Core.Application.Dto.Patient
{
    public class PatientDetailDto
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string IdentificationType { get; set; } = string.Empty;
        public string PatientType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<MedicalRecordSummaryDto> MedicalRecords { get; set; } = [];
        public List<MedicalDocumentDto> Documents { get; set; } = [];
    }
}
