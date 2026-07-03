using AutoMapper;
using MedAnalyzer.Core.Application.Dto.Recommendation;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Mappers.Recommendations
{
    public class RecommendationDtoMappingProfile : Profile
    {
        public RecommendationDtoMappingProfile()
        {
            CreateMap<Recommendation, RecommendationDto>().ReverseMap();
        }
    }
}
