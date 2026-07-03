using MedAnalyzer.Core.Application.Dto.Dashboard;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardDto> GetDashboardStatsAsync();
    }
}
