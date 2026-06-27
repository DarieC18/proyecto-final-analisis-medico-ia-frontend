using MedAnalyzer.Core.Application.Dto.Dashboard;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Interfaces;
using MedAnalyzer.Infraestructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MedAnalyzer.Infraestructure.Identity.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IBaseRepository<Patient> _patientRepo;
        private readonly IBaseRepository<Appointment> _appointmentRepo;
        private readonly IBaseRepository<AiAnalysis> _aiAnalysisRepo;
        private readonly IBaseRepository<Alert> _alertRepo;
        private readonly IAuditLogService _auditLogService;

        public AdminDashboardService(
            UserManager<AppUser> userManager,
            IBaseRepository<Patient> patientRepo,
            IBaseRepository<Appointment> appointmentRepo,
            IBaseRepository<AiAnalysis> aiAnalysisRepo,
            IBaseRepository<Alert> alertRepo,
            IAuditLogService auditLogService)
        {
            _userManager = userManager;
            _patientRepo = patientRepo;
            _appointmentRepo = appointmentRepo;
            _aiAnalysisRepo = aiAnalysisRepo;
            _alertRepo = alertRepo;
            _auditLogService = auditLogService;
        }

        public async Task<AdminDashboardDto> GetDashboardStatsAsync()
        {
            var totalUsers = await _userManager.Users.CountAsync();
            var totalActiveUsers = await _userManager.Users.CountAsync(u => u.Status && u.EmailConfirmed);

            var appointmentsByStatus = await _appointmentRepo.GetAllQuery()
                .GroupBy(a => a.Status)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToListAsync();

            var latestLogs = await _auditLogService.GetFilteredAsync(null, null, null, null);

            return new AdminDashboardDto
            {
                TotalUsers = totalUsers,
                TotalActiveUsers = totalActiveUsers,
                TotalInactiveUsers = totalUsers - totalActiveUsers,
                TotalPatients = await _patientRepo.GetAllQuery().CountAsync(),
                AppointmentsByStatus = appointmentsByStatus.ToDictionary(x => x.Key, x => x.Count),
                TotalAiAnalyses = await _aiAnalysisRepo.GetAllQuery().CountAsync(),
                ActiveAlerts = await _alertRepo.GetAllQuery().CountAsync(a => !a.IsResolved),
                LatestAuditLogs = latestLogs.Take(10).ToList()
            };
        }
    }
}
