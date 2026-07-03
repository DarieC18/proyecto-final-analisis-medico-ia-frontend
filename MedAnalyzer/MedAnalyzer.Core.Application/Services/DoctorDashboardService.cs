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
        public DoctorDashboardService(IAlertService alert, IAiAnalisysServices aiAnalysis,
            IAppointmentService appointment, IPatientService patient)
        {
            _alertService = alert;
            _aiAnalysis = aiAnalysis;
            _appointment = appointment;
            _patient = patient;

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
                    && a.AppointmentDate == DateTime.Now.Date);

                var totalAiAnalysesByDoctor = totalAiAnalyses.Where(a => a.RequestedByUserId == id);
                
                

                return new DoctorDashboardDto
                {
                    TotalPatients = totalPatients.Count(),

                    ActiveAlerts = allAlerts.Count(a => !a.IsResolved),

                    TotalAppointmentsToday = totalAppointmentsToday.Count(),

                    CompletedAppointmentsToday = totalAppointmentsToday
                        .Where(a => a.Status == AppointmentStatus.Completed.ToString()).Count(),

                    PendingAppointmentsToday = totalAppointmentsToday
                        .Where(a => a.Status == AppointmentStatus.Pending.ToString()).Count(),

                    TotalAiAnalyses = totalAiAnalysesByDoctor.Count(),

                    ApprovedAiAnalyses = totalAiAnalysesByDoctor
                        .Where(a => a.Status == AiAnalysisStatus.Approved.ToString()).Count(),

                    PendingAiAnalyses = totalAiAnalysesByDoctor
                        .Where(a => a.Status == AiAnalysisStatus.Pending.ToString()).Count(),

                    RejectedAiAnalyses = totalAiAnalysesByDoctor
                        .Where(a => a.Status == AiAnalysisStatus.Rejected.ToString()).Count()

                };

            }
            catch (Exception ex)
            {

                throw new ApplicationException("An error occurred while fetching dashboard data.", ex);

            }
        }
    }
}
