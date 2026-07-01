using MedAnalyzer.Core.Application.Dto.Recommendation;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IRecommendationService : IBaseServices<Recommendation, RecommendationDto>
    {
        Task<List<RecommendationDto>> GetByAppointmentId(int appointmentId);
        Task<List<RecommendationDto>> GetByPatientId(int patientId);
    }
}
