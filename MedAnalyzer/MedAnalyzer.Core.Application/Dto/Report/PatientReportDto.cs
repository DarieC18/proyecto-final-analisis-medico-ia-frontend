using MedAnalyzer.Core.Application.Dto.Alert;
using MedAnalyzer.Core.Application.Dto.Patient;

namespace MedAnalyzer.Core.Application.Dto.Report
{
    public class PatientReportDto
    {
        public PatientDetailDto? PatientDetail { get; set; }
        public List<AlertDto> Alerts { get; set; } = [];
    }
}
