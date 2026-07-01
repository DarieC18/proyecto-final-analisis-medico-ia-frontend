using MedAnalyzer.Core.Application.Base;

namespace MedAnalyzer.Core.Application.Dto.Alert
{
    public class AlertDto : BaseDto<int>
    {
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Severity { get; set; }
        public bool IsResolved { get; set; }
    }
}
