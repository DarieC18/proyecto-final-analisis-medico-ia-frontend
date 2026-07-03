using AutoMapper;
using MedAnalyzer.Core.Application.Base;
using MedAnalyzer.Core.Application.Dto.Recommendation;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Interfaces;

namespace MedAnalyzer.Core.Application.Services
{
    public class RecommendationService : BaseServices<Recommendation, RecommendationDto>, IRecommendationService
    {
        private readonly IBaseRepository<Recommendation> _repository;
        private readonly IBaseRepository<AiAnalysis> _aiAnalysisRepository;
        private readonly IMapper _mapper;

        public RecommendationService(
            IMapper mapper,
            IBaseRepository<Recommendation> repository,
            IBaseRepository<AiAnalysis> aiAnalysisRepository)
            : base(mapper, repository)
        {
            _repository = repository;
            _aiAnalysisRepository = aiAnalysisRepository;
            _mapper = mapper;
        }

        public async Task<List<RecommendationDto>> GetByAppointmentId(int appointmentId)
        {
            var all = await _repository.GetAllListAsync();
            return _mapper.Map<List<RecommendationDto>>(
                all.Where(r => r.AppointmentId == appointmentId).ToList());
        }

        public async Task<List<RecommendationDto>> GetByPatientId(int patientId)
        {
            var allAnalyses = await _aiAnalysisRepository.GetAllListAsync();
            var patientAnalysisIds = allAnalyses
                .Where(a => a.PatientId == patientId)
                .Select(a => a.Id)
                .ToHashSet();

            var all = await _repository.GetAllListAsync();
            return _mapper.Map<List<RecommendationDto>>(
                all.Where(r => patientAnalysisIds.Contains(r.AiAnalysisId)).ToList());
        }
    }
}
