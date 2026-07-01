using MedAnalyzer.Core.Application.Dto.AiAnalisys;
using MedAnalyzer.Core.Application.Dto.Appointment;
using MedAnalyzer.Core.Application.Dto.Recommendation;
using MedAnalyzer.Core.Application.Dto.Report;
using MedAnalyzer.Core.Application.Interfaces;

namespace MedAnalyzer.Core.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        private readonly IAlertService _alertService;
        private readonly IAiAnalisysServices _aiService;
        private readonly IRecommendationService _recommendationService;

        public ReportService(
            IAppointmentService appointmentService,
            IPatientService patientService,
            IAlertService alertService,
            IAiAnalisysServices aiService,
            IRecommendationService recommendationService)
        {
            _appointmentService = appointmentService;
            _patientService = patientService;
            _alertService = alertService;
            _aiService = aiService;
            _recommendationService = recommendationService;
        }

        public async Task<AppointmentDetailDto?> GetAppointmentReportAsync(int appointmentId)
            => await _appointmentService.GetAppointmentDetail(appointmentId);

        public async Task<PatientReportDto?> GetPatientReportAsync(int patientId)
        {
            var detail = await _patientService.GetPatientDetail(patientId);
            if (detail == null) return null;

            var alerts = await _alertService.GetByPatientIdAsync(patientId);

            return new PatientReportDto { PatientDetail = detail, Alerts = alerts };
        }

        public async Task<List<AiAnalisysDto>> GetAiAnalysesReportAsync(int appointmentId)
            => await _aiService.GetByAppointmentIdAsync(appointmentId);

        public async Task<List<RecommendationDto>> GetRecommendationsReportAsync(int patientId)
            => await _recommendationService.GetByPatientId(patientId);
    }
}
