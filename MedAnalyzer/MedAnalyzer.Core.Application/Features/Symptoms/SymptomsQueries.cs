using MedAnalyzer.Core.Application.Dto.Symptom;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.Symptoms.Queries
{
    public record GetSymptomsByAppointmentQuery(int AppointmentId) : IRequest<List<SymptomDto>>;
    public class GetSymptomsByAppointmentQueryHandler : IRequestHandler<GetSymptomsByAppointmentQuery, List<SymptomDto>>
    {
        private readonly ISymptomService _service;
        public GetSymptomsByAppointmentQueryHandler(ISymptomService service) => _service = service;
        public async Task<List<SymptomDto>> Handle(GetSymptomsByAppointmentQuery req, CancellationToken ct)
            => await _service.GetByAppointmentId(req.AppointmentId) ?? [];
    }

    public record GetSymptomByIdQuery(int Id) : IRequest<SymptomDto?>;
    public class GetSymptomByIdQueryHandler : IRequestHandler<GetSymptomByIdQuery, SymptomDto?>
    {
        private readonly ISymptomService _service;
        public GetSymptomByIdQueryHandler(ISymptomService service) => _service = service;
        public async Task<SymptomDto?> Handle(GetSymptomByIdQuery req, CancellationToken ct)
            => await _service.GetDtoById(req.Id);
    }
}
