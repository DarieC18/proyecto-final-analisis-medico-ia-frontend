using AutoMapper;
using MedAnalyzer.Core.Application.Dto.MedicalRecord;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Mappers.MedicalRecords
{
    public class MedicalRecordDtoMappingProfile : Profile
    {
        public MedicalRecordDtoMappingProfile()
        {
            CreateMap<MedicalRecord, MedicalRecordDto>().ReverseMap();

            CreateMap<MedicalRecord, MedicalRecordSummaryDto>()
                .ForMember(d => d.AppointmentDate, o => o.MapFrom(s => s.Appointment != null ? s.Appointment.AppointmentDate : default))
                .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Appointment != null ? s.Appointment.DoctorId : string.Empty))
                .ForMember(d => d.Reason, o => o.MapFrom(s => s.Appointment != null ? s.Appointment.Reason : string.Empty))
                .ForMember(d => d.AppointmentStatus, o => o.MapFrom(s => s.Appointment != null ? s.Appointment.Status : string.Empty));
        }
    }
}
