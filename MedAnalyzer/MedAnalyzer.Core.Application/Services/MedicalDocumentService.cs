using AutoMapper;
using MedAnalyzer.Core.Application.Base;
using MedAnalyzer.Core.Application.Dto.MedicalDocument;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Interfaces;

namespace MedAnalyzer.Core.Application.Services
{
    public class MedicalDocumentService : BaseServices<MedicalDocument, MedicalDocumentDto>, IMedicalDocumentService
    {
        private readonly IBaseRepository<MedicalDocument> _repository;
        private readonly IMapper _mapper;

        public MedicalDocumentService(IMapper mapper, IBaseRepository<MedicalDocument> repository)
            : base(mapper, repository)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<MedicalDocumentDto>> GetByPatientId(int patientId)
        {
            var all = await _repository.GetAllListAsync();
            return _mapper.Map<List<MedicalDocumentDto>>(
                all.Where(d => d.PatientId == patientId).ToList());
        }

        public async Task<List<MedicalDocumentDto>> GetByAppointmentId(int appointmentId)
        {
            var all = await _repository.GetAllListAsync();
            return _mapper.Map<List<MedicalDocumentDto>>(
                all.Where(d => d.AppointmentId == appointmentId).ToList());
        }
    }
}
