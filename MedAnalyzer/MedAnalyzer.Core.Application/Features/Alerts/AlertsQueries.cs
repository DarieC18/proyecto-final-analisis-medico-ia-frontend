using MedAnalyzer.Core.Application.Dto.Alert;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.Alerts.Queries
{
    public record GetAlertsByPatientQuery(int PatientId) : IRequest<List<AlertDto>>;
    public class GetAlertsByPatientQueryHandler : IRequestHandler<GetAlertsByPatientQuery, List<AlertDto>>
    {
        private readonly IAlertService _service;
        public GetAlertsByPatientQueryHandler(IAlertService service) => _service = service;
        public async Task<List<AlertDto>> Handle(GetAlertsByPatientQuery req, CancellationToken ct)
            => await _service.GetByPatientIdAsync(req.PatientId);
    }

    public record GetAlertsByAppointmentQuery(int AppointmentId) : IRequest<List<AlertDto>>;
    public class GetAlertsByAppointmentQueryHandler : IRequestHandler<GetAlertsByAppointmentQuery, List<AlertDto>>
    {
        private readonly IAlertService _service;
        public GetAlertsByAppointmentQueryHandler(IAlertService service) => _service = service;
        public async Task<List<AlertDto>> Handle(GetAlertsByAppointmentQuery req, CancellationToken ct)
            => await _service.GetByAppointmentIdAsync(req.AppointmentId);
    }

    public record GetActiveAlertsQuery : IRequest<List<AlertDto>>;
    public class GetActiveAlertsQueryHandler : IRequestHandler<GetActiveAlertsQuery, List<AlertDto>>
    {
        private readonly IAlertService _service;
        public GetActiveAlertsQueryHandler(IAlertService service) => _service = service;
        public async Task<List<AlertDto>> Handle(GetActiveAlertsQuery _, CancellationToken ct)
            => await _service.GetActiveAsync();
    }
}
