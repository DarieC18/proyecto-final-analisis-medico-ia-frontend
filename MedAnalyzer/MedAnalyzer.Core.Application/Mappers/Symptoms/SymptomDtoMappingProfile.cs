using AutoMapper;
using MedAnalyzer.Core.Application.Dto.Symptom;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Mappers.Symptoms
{
    public class SymptomDtoMappingProfile : Profile
    {
        public SymptomDtoMappingProfile()
        {
            CreateMap<Symptom, SymptomDto>().ReverseMap();
        }
    }
}
