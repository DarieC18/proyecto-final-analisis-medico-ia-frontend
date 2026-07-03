using AutoMapper;
using MedAnalyzer.Core.Application.Dto.Appointment;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Mappers.Appointments
{
    public class AppointmentDtoMappingProfile : Profile
    {
        public AppointmentDtoMappingProfile() 
        { 
            CreateMap<Appointment,AppointmentDto>().ReverseMap();

            CreateMap<Appointment, AppointmentSummaryDto>()
                .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.DoctorId));
        }
    }
}
