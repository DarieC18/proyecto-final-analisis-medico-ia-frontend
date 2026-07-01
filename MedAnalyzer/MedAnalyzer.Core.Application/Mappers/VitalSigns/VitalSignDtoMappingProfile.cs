using AutoMapper;
using MedAnalyzer.Core.Application.Dto.VitalSign;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Mappers.VitalSigns
{
    public class VitalSignDtoMappingProfile : Profile
    {
        public VitalSignDtoMappingProfile()
        {
            CreateMap<VitalSign, VitalSignDto>().ReverseMap();
        }
    }
}
