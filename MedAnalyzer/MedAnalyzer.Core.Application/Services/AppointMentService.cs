using AutoMapper;
using MedAnalyzer.Core.Application.Base;
using MedAnalyzer.Core.Application.Dto.AiAnalisys;
using MedAnalyzer.Core.Application.Dto.Appointment;
using MedAnalyzer.Core.Application.Dto.MedicalDocument;
using MedAnalyzer.Core.Application.Dto.MedicalRecord;
using MedAnalyzer.Core.Application.Dto.Symptom;
using MedAnalyzer.Core.Application.Dto.VitalSign;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Enum;
using MedAnalyzer.Core.Domain.Exceptions;
using MedAnalyzer.Core.Domain.Interfaces;

namespace MedAnalyzer.Core.Application.Services
{
    public class AppointMentService : BaseServices<Appointment, AppointmentDto>, IAppointmentService
    {
        private readonly IBaseRepository<Appointment> _appointmentRepository;
        private readonly IBaseRepository<MedicalRecord> _medicalRecordRepository;
        private readonly IBaseRepository<Symptom> _symptomRepository;
        private readonly IBaseRepository<VitalSign> _vitalSignRepository;
        private readonly IBaseRepository<MedicalDocument> _medicalDocumentRepository;
        private readonly IBaseRepository<AiAnalysis> _aiAnalysisRepository;
        private readonly IMapper _mapper;

        public AppointMentService(
            IMapper mapper,
            IBaseRepository<Appointment> repository,
            IBaseRepository<MedicalRecord> medicalRecordRepository,
            IBaseRepository<Symptom> symptomRepository,
            IBaseRepository<VitalSign> vitalSignRepository,
            IBaseRepository<MedicalDocument> medicalDocumentRepository,
            IBaseRepository<AiAnalysis> aiAnalysisRepository) : base(mapper, repository)
        {
            _appointmentRepository = repository;
            _medicalRecordRepository = medicalRecordRepository;
            _symptomRepository = symptomRepository;
            _vitalSignRepository = vitalSignRepository;
            _medicalDocumentRepository = medicalDocumentRepository;
            _aiAnalysisRepository = aiAnalysisRepository;
            _mapper = mapper;
        }

        private static readonly string[] ValidStatuses = Enum.GetNames(typeof(AppointmentStatus));

        private static void Validate(AppointmentDto dto)
        {
            if (dto.PatientId == 0)
                throw new DomainValidationException("El paciente es requerido.");
            if (dto.AppointmentDate < DateTime.Now)
                throw new DomainValidationException("La fecha de la cita no puede ser una fecha pasada.");
            if (!ValidStatuses.Contains(dto.Status, StringComparer.OrdinalIgnoreCase))
                throw new DomainValidationException($"Estado no válido. Use: {string.Join(", ", ValidStatuses)}.");
        }

        public override async Task<AppointmentDto?> SaveDtoAsync(AppointmentDto dto)
        {
            Validate(dto);
            return await base.SaveDtoAsync(dto);
        }

        public override async Task<AppointmentDto?> UpdateDtoAsync(AppointmentDto dto, int id)
        {
            Validate(dto);
            return await base.UpdateDtoAsync(dto, id);
        }

        public async Task<AppointmentDto?> ChangeStatusAsync(int id, string status)
        {
            if (!Enum.TryParse<AppointmentStatus>(status, true, out _))
                throw new DomainValidationException("Estado no válido. Use: Pending, InProgress, Completed, Cancelled.");

            var appointment = await _appointmentRepository.GetEntityByIdAsync(id);
            if (appointment == null) return null;

            appointment.Status = status;
            var updated = await _appointmentRepository.UpdateEntityAsync(id, appointment);
            return updated == null ? null : _mapper.Map<AppointmentDto>(updated);
        }

        public async Task<AppointmentDetailDto?> GetAppointmentDetail(int id)
        {
            var appointment = await _appointmentRepository.GetEntityByIdAsync(id);
            if (appointment == null) return null;

            var allRecords = await _medicalRecordRepository.GetAllListAsync();
            var allSymptoms = await _symptomRepository.GetAllListAsync();
            var allVitalSigns = await _vitalSignRepository.GetAllListAsync();
            var allDocuments = await _medicalDocumentRepository.GetAllListAsync();
            var allAiAnalyses = await _aiAnalysisRepository.GetAllListAsync();

            return new AppointmentDetailDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                AppointmentDate = appointment.AppointmentDate,
                Status = appointment.Status,
                Reason = appointment.Reason,
                Notes = appointment.Notes,
                CreatedAt = appointment.CreatedAt,
                MedicalRecords = _mapper.Map<List<MedicalRecordDto>>(
                    allRecords.Where(r => r.AppointmentId == id).ToList()),
                Symptoms = _mapper.Map<List<SymptomDto>>(
                    allSymptoms.Where(s => s.AppointmentId == id).ToList()),
                VitalSigns = _mapper.Map<List<VitalSignDto>>(
                    allVitalSigns.Where(v => v.AppointmentId == id).ToList()),
                Documents = _mapper.Map<List<MedicalDocumentDto>>(
                    allDocuments.Where(d => d.AppointmentId == id).ToList()),
                AiAnalyses = _mapper.Map<List<AiAnalisysDto>>(
                    allAiAnalyses.Where(a => a.AppointmentId == id).ToList())
            };
        }
    }
}
