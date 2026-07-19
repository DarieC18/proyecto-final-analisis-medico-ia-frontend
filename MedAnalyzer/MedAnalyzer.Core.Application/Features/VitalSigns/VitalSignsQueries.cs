using MedAnalyzer.Core.Application.Dto.VitalSign;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.VitalSigns.Queries
{
    public record GetVitalSignsByAppointmentQuery(int AppointmentId) : IRequest<List<VitalSignDto>>;
    public class GetVitalSignsByAppointmentQueryHandler : IRequestHandler<GetVitalSignsByAppointmentQuery, List<VitalSignDto>>
    {
        private readonly IVitalSignService _service;
        public GetVitalSignsByAppointmentQueryHandler(IVitalSignService service) => _service = service;
        public async Task<List<VitalSignDto>> Handle(GetVitalSignsByAppointmentQuery req, CancellationToken ct)
            => await _service.GetByAppointmentId(req.AppointmentId) ?? [];
    }

    public record GetVitalSignByIdQuery(int Id) : IRequest<VitalSignDto?>;
    public class GetVitalSignByIdQueryHandler : IRequestHandler<GetVitalSignByIdQuery, VitalSignDto?>
    {
        private readonly IVitalSignService _service;
        public GetVitalSignByIdQueryHandler(IVitalSignService service) => _service = service;
        public async Task<VitalSignDto?> Handle(GetVitalSignByIdQuery req, CancellationToken ct)
            => await _service.GetDtoById(req.Id);
    }
}
