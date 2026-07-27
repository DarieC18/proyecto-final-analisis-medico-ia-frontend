using MedAnalyzer.Core.Application.Dto.Appointment;
using MedAnalyzer.Core.Application.Dto.Dashboard;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Enum;

namespace MedAnalyzer.Core.Application.Services
{
    public class DoctorDashboardService : IDashboardService<DoctorDashboardDto>
    {
        private readonly IAlertService _alertService;
        private readonly IAiAnalisysServices _aiAnalysis;
        private readonly IAppointmentService _appointment;
        private readonly IPatientService _patient;
        private readonly IVitalSignService _vitalSignService;
        private readonly ISymptomService _symptomService;

        public DoctorDashboardService(IAlertService alert, IAiAnalisysServices aiAnalysis,
            IAppointmentService appointment, IPatientService patient,
            IVitalSignService vitalSignService, ISymptomService symptomService)
        {
            _alertService = alert;
            _aiAnalysis = aiAnalysis;
            _appointment = appointment;
            _patient = patient;
            _vitalSignService = vitalSignService;
            _symptomService = symptomService;
        }

        public async Task<DoctorDashboardDto> GetDashboard(string id)
        {
            try
            {
                var totalPatients = await _patient.GetAllListDto();
                var totalAppointments = await _appointment.GetAllListDto();
                var totalAiAnalyses = await _aiAnalysis.GetAllListDto();
                var allAlerts = await _alertService.GetAllListDto();

                var totalAppointmentsToday = totalAppointments.Where(a => a.DoctorId == id 
                    && a.AppointmentDate.Date == DateTime.Now.Date);

                var totalAiAnalysesByDoctor = totalAiAnalyses.Where(a => a.RequestedByUserId == id);

                var nurseAppointmentIds = totalAppointments
                    .Where(a => a.DoctorId == id)
                    .Select(a => a.Id)
                    .ToHashSet();

                var allVitalSigns = await _vitalSignService.GetAllListDto();
                var latestVitalSigns = allVitalSigns
                    .Where(v => nurseAppointmentIds.Contains(v.AppointmentId))
                    .OrderByDescending(v => v.MeasuredAt)
                    .Take(5)
                    .ToList();

                var allSymptoms = await _symptomService.GetAllListDto();
                var latestSymptoms = allSymptoms
                    .Where(s => nurseAppointmentIds.Contains(s.AppointmentId))
                    .OrderByDescending(s => s.StartedAt)
                    .Take(5)
                    .ToList();

                return new DoctorDashboardDto
                {
                    TotalPatients = totalPatients.Count(),
                    ActiveAlerts = allAlerts.Count(a => !a.IsResolved),
                    TotalAppointmentsToday = totalAppointmentsToday.Count(),
                    CompletedAppointmentsToday = totalAppointmentsToday
                        .Count(a => a.Status == AppointmentStatus.Completed.ToString()),
                    PendingAppointmentsToday = totalAppointmentsToday
                        .Count(a => a.Status == AppointmentStatus.Pending.ToString()),
                    TotalAiAnalyses = totalAiAnalysesByDoctor.Count(),
                    ApprovedAiAnalyses = totalAiAnalysesByDoctor
                        .Count(a => a.Status == AiAnalysisStatus.Approved.ToString()),
                    PendingAiAnalyses = totalAiAnalysesByDoctor
                        .Count(a => a.Status == AiAnalysisStatus.Pending.ToString()),
                    RejectedAiAnalyses = totalAiAnalysesByDoctor
                        .Count(a => a.Status == AiAnalysisStatus.Rejected.ToString()),
                    LatestVitalSigns = latestVitalSigns,
                    LatestSymptoms = latestSymptoms
                };

            }
            catch (Exception ex)
            {

                throw new ApplicationException("An error occurred while fetching dashboard data.", ex);

            }
        }
    }
}
