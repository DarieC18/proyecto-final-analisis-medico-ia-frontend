using MedAnalyzer.Core.Application.Dto.MedicalDocument;
using MedAnalyzer.Core.Application.Dto.MedicalRecord;
using MedAnalyzer.Core.Application.Dto.Symptom;
using MedAnalyzer.Core.Application.Dto.VitalSign;

namespace MedAnalyzer.Core.Application.Dto.Appointment
{
    public class AppointmentConsultDto
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public required string DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public required string Reason { get; set; }
        public required string Status { get; set; }

        public List<SymptomDto> Symptoms { get; set; } = [];

        public List<VitalSignDto> VitalSigns { get; set; } = [];

        public List<MedicalRecordDto> MedicalRecords { get; set; } = [];

        public List<MedicalDocumentDto> Documents { get; set; } = [];
    }
}