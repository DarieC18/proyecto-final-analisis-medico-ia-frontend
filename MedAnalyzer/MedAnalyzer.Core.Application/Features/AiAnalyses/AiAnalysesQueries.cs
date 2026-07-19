using MedAnalyzer.Core.Application.Dto.AiAnalisys;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.AiAnalyses.Queries
{
    public record GetAllAiAnalysesQuery : IRequest<List<AiAnalisysDto>>;
    public class GetAllAiAnalysesQueryHandler : IRequestHandler<GetAllAiAnalysesQuery, List<AiAnalisysDto>>
    {
        private readonly IAiAnalisysServices _service;
        public GetAllAiAnalysesQueryHandler(IAiAnalisysServices service) => _service = service;
        public async Task<List<AiAnalisysDto>> Handle(GetAllAiAnalysesQuery _, CancellationToken ct)
            => await _service.GetAllListDto();
    }

    public record GetAiAnalysesByAppointmentQuery(int AppointmentId) : IRequest<List<AiAnalisysDto>>;
    public class GetAiAnalysesByAppointmentQueryHandler : IRequestHandler<GetAiAnalysesByAppointmentQuery, List<AiAnalisysDto>>
    {
        private readonly IAiAnalisysServices _service;
        public GetAiAnalysesByAppointmentQueryHandler(IAiAnalisysServices service) => _service = service;
        public async Task<List<AiAnalisysDto>> Handle(GetAiAnalysesByAppointmentQuery req, CancellationToken ct)
            => await _service.GetByAppointmentIdAsync(req.AppointmentId);
    }

    public record GetAiAnalysesByPatientQuery(int PatientId) : IRequest<List<AiAnalisysDto>>;
    public class GetAiAnalysesByPatientQueryHandler : IRequestHandler<GetAiAnalysesByPatientQuery, List<AiAnalisysDto>>
    {
        private readonly IAiAnalisysServices _service;
        public GetAiAnalysesByPatientQueryHandler(IAiAnalisysServices service) => _service = service;
        public async Task<List<AiAnalisysDto>> Handle(GetAiAnalysesByPatientQuery req, CancellationToken ct)
            => await _service.GetByPatientIdAsync(req.PatientId);
    }

    public record GetAiAnalysisByIdQuery(int Id) : IRequest<AiAnalisysDto?>;
    public class GetAiAnalysisByIdQueryHandler : IRequestHandler<GetAiAnalysisByIdQuery, AiAnalisysDto?>
    {
        private readonly IAiAnalisysServices _service;
        public GetAiAnalysisByIdQueryHandler(IAiAnalisysServices service) => _service = service;
        public async Task<AiAnalisysDto?> Handle(GetAiAnalysisByIdQuery req, CancellationToken ct)
            => await _service.GetDtoById(req.Id);
    }
}
