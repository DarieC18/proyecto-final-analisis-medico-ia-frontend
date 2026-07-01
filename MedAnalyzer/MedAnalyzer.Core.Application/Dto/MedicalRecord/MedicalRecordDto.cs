
namespace MedAnalyzer.Core.Application.Dto.MedicalRecord
{
    public class MedicalRecordDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }
        public required string CreatedByUserId { get; set; }
        public required string DiagnosisInitial { get; set; }
        public required string Notes { get; set; }
        public string? Antecedentes { get; set; }
        public string? ObservacionesConsulta { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
