using AutoMapper;
using MedAnalyzer.Core.Application.Base;
using MedAnalyzer.Core.Application.Dto.MedicalDocument;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedAnalyzer.Core.Application.Services
{
    public class MedicalDocumentService : BaseServices<MedicalDocument, MedicalDocumentDto>, IMedicalDocumentService
    {
        private readonly IBaseRepository<MedicalDocument> _repository;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        public MedicalDocumentService(IMapper mapper, IBaseRepository<MedicalDocument> repository, IAuditLogService auditLogService)
            : base(mapper, repository)
        {
            _repository = repository;
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        public async Task<List<MedicalDocumentDto>> GetByPatientId(int patientId)
        {
            var docs = _repository.GetAllQuery().Where(d => d.PatientId == patientId).OrderByDescending(d => d.UploadedAt).ToList();

            var dtos = _mapper.Map<List<MedicalDocumentDto>>(docs);

            return dtos;
        }

        public async Task<List<MedicalDocumentDto>> GetByAppointmentId(int appointmentId)
        {
            var all = await _repository.GetAllListAsync();
            return _mapper.Map<List<MedicalDocumentDto>>(
                all.Where(d => d.AppointmentId == appointmentId).ToList());
        }

        public async Task<MedicalDocumentDto?> UploadDocument(MedicalDocumentDto dto, string currentUserId)
        {
            var result = await SaveDtoAsync(dto);

            if (result != null)
            {
                await _auditLogService.LogAsync(currentUserId, "UploadDocument", "MedicalDocument", result.Id.ToString());
            }

            return result;
        }

        public async Task<bool> DeleteDocumentAsync(int id, string currentUserId)
        {
            var entity = await _repository.GetEntityByIdAsync(id);
            if (entity == null) return false;
            try
            {
                await _repository.RemoveAsync(id);
                await _auditLogService.LogAsync(currentUserId, "DeleteDocument", "MedicalDocument", id.ToString());
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }
    }
}
