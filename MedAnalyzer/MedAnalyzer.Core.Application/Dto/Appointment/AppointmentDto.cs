using MedAnalyzer.Core.Application.Base;

namespace MedAnalyzer.Core.Application.Dto.Appointment
{
    public class AppointmentDto : BaseDto<int>
    {
        public int PatientId { get; set; }
        public string? DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = "Pending";
        public required string Reason { get; set; }
        public string? Notes { get; set; }
    }
}
