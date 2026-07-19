using MedAnalyzer.Core.Application.Dto.Recommendation;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.Recommendations.Queries
{
    public record GetRecommendationsByAppointmentQuery(int AppointmentId) : IRequest<List<RecommendationDto>>;
    public class GetRecommendationsByAppointmentQueryHandler : IRequestHandler<GetRecommendationsByAppointmentQuery, List<RecommendationDto>>
    {
        private readonly IRecommendationService _service;
        public GetRecommendationsByAppointmentQueryHandler(IRecommendationService service) => _service = service;
        public async Task<List<RecommendationDto>> Handle(GetRecommendationsByAppointmentQuery req, CancellationToken ct)
            => await _service.GetByAppointmentId(req.AppointmentId);
    }

    public record GetRecommendationsByPatientQuery(int PatientId) : IRequest<List<RecommendationDto>>;
    public class GetRecommendationsByPatientQueryHandler : IRequestHandler<GetRecommendationsByPatientQuery, List<RecommendationDto>>
    {
        private readonly IRecommendationService _service;
        public GetRecommendationsByPatientQueryHandler(IRecommendationService service) => _service = service;
        public async Task<List<RecommendationDto>> Handle(GetRecommendationsByPatientQuery req, CancellationToken ct)
            => await _service.GetByPatientId(req.PatientId);
    }

    public record GetRecommendationByIdQuery(int Id) : IRequest<RecommendationDto?>;
    public class GetRecommendationByIdQueryHandler : IRequestHandler<GetRecommendationByIdQuery, RecommendationDto?>
    {
        private readonly IRecommendationService _service;
        public GetRecommendationByIdQueryHandler(IRecommendationService service) => _service = service;
        public async Task<RecommendationDto?> Handle(GetRecommendationByIdQuery req, CancellationToken ct)
            => await _service.GetDtoById(req.Id);
    }
}
