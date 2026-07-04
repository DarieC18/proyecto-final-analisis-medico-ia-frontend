using AutoMapper;
using MedAnalyzer.Core.Application.Dto.Alert;
using MedAnalyzer.Core.Domain.Entities;
namespace MedAnalyzer.Core.Application.Mappers.Alerts
{
    public class AlertDtoMappingProfile : Profile
    {
       public AlertDtoMappingProfile() 
        { 
            CreateMap<Alert,AlertDto>().ReverseMap();
        }
    }
}
