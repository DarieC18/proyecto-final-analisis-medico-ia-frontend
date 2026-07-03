using MedAnalyzer.Core.Application.Dto.AiAnalisys;
using MedAnalyzer.Core.Application.Dto.Appointment;
using MedAnalyzer.Core.Application.Dto.Recommendation;
using MedAnalyzer.Core.Application.Dto.Report;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IReportService
    {
        Task<AppointmentDetailDto?> GetAppointmentReportAsync(int appointmentId);
        Task<PatientReportDto?> GetPatientReportAsync(int patientId);
        Task<List<AiAnalisysDto>> GetAiAnalysesReportAsync(int appointmentId);
        Task<List<RecommendationDto>> GetRecommendationsReportAsync(int patientId);
    }
}
