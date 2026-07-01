
namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IDashboardService<TDashboardDto>
        where TDashboardDto : class
    {
        public Task<TDashboardDto> GetDashboard(string id);
    }
}
