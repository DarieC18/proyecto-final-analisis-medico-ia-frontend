using AutoMapper;
using MedAnalyzer.Core.Application.Dto.Patient;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Mappers.Patients
{
    public class PatientDetailDtoMappingProfile : Profile
    {
        public PatientDetailDtoMappingProfile()
        {
            CreateMap<Patient, PatientDetailDto>()
                .ForMember(d => d.FullName, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.UserId));
        }
    }
}
