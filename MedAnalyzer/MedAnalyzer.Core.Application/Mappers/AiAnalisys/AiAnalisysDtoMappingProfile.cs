using AutoMapper;
using MedAnalyzer.Core.Application.Dto.AiAnalisys;
using MedAnalyzer.Core.Domain.Entities;
namespace MedAnalyzer.Core.Application.Mappers.AiAnalisys
{
    public class AiAnalisysDtoMappingProfile : Profile
    {
        public AiAnalisysDtoMappingProfile()
        {
            CreateMap<AiAnalysis,AiAnalisysDto>().ReverseMap();
        }
    }
}
