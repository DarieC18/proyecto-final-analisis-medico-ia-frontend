using MedAnalyzer.Core.Application.Dto.Dashboard;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.Dashboard.Queries
{
    public record GetAdminDashboardQuery : IRequest<AdminDashboardDto>;
    public class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
    {
        private readonly IAdminDashboardService _service;
        public GetAdminDashboardQueryHandler(IAdminDashboardService service) => _service = service;
        public async Task<AdminDashboardDto> Handle(GetAdminDashboardQuery _, CancellationToken ct)
            => await _service.GetDashboardStatsAsync();
    }

    public record GetDoctorDashboardQuery(string UserId) : IRequest<DoctorDashboardDto?>;
    public class GetDoctorDashboardQueryHandler : IRequestHandler<GetDoctorDashboardQuery, DoctorDashboardDto?>
    {
        private readonly IDashboardService<DoctorDashboardDto> _service;
        public GetDoctorDashboardQueryHandler(IDashboardService<DoctorDashboardDto> service) => _service = service;
        public async Task<DoctorDashboardDto?> Handle(GetDoctorDashboardQuery req, CancellationToken ct)
            => await _service.GetDashboard(req.UserId);
    }
}
