using AutoMapper;
using MedAnalyzer.Core.Application.Dto.Patient;
using MedAnalyzer.Core.Domain.Entities;
namespace MedAnalyzer.Core.Application.Mappers.Patients
{
    public class PatientDtoMappingProfile : Profile
    {
        public PatientDtoMappingProfile() 
        {
              CreateMap<Patient, PatientDto>().ReverseMap();
        }
    }
}
