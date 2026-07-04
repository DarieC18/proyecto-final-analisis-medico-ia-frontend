using AutoMapper;
using MedAnalyzer.Core.Application.Dto.Patient;
using MedAnalyzer.Core.Domain.Entities;
namespace MedAnalyzer.Core.Application.Mappers.Patients
{
    public class PatientDetailDtoMappingProfile : Profile
    {
        public PatientDetailDtoMappingProfile()
        {
            CreateMap<PatientDetailDto,Patient>().ReverseMap();
        }
    }
}
