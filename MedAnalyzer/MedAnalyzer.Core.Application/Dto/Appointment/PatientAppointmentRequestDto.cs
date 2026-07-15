namespace MedAnalyzer.Core.Application.Dto.Appointment
{
    public class PatientAppointmentRequestDto
    {
        public string? DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public required string Reason { get; set; }
        public string? Notes { get; set; }
    }
}
