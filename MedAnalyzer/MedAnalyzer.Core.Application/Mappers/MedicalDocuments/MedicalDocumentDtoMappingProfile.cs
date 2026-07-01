using AutoMapper;
using MedAnalyzer.Core.Application.Dto.MedicalDocument;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Mappers.MedicalDocuments
{
    public class MedicalDocumentDtoMappingProfile : Profile
    {
        public MedicalDocumentDtoMappingProfile()
        {
            CreateMap<MedicalDocument, MedicalDocumentDto>().ReverseMap();
        }
    }
}
