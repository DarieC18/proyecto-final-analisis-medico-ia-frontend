using AutoMapper;
using MedAnalyzer.Core.Application.Dto.AuditLog;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Mappers.AuditLogs
{
    public class AuditLogDtoMappingProfile : Profile
    {
        public AuditLogDtoMappingProfile()
        {
            CreateMap<AuditLog, AuditLogDto>();
            CreateMap<AuditLogDto, AuditLog>();
        }
    }
}