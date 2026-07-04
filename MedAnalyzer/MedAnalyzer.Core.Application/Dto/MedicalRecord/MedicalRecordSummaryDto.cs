
namespace MedAnalyzer.Core.Application.Dto.MedicalRecord
{
    public class MedicalRecordSummaryDto
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public required string DoctorName { get; set; }
        public required string Reason { get; set; }
        public required string AppointmentStatus { get; set; }
    }
}
