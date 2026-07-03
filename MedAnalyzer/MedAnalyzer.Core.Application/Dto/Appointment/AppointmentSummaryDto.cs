namespace MedAnalyzer.Core.Application.Dto.Appointment
{
    public class AppointmentSummaryDto
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public required string Reason { get; set; }
        public required string Status { get; set; }
    }
}
