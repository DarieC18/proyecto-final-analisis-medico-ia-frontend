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
using Microsoft.EntityFrameworkCore;

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
        private readonly IPatientRepository _patientRepository;
        private readonly IBaseAccountService _accountService;
        private readonly IAuditLogService _auditLogService;
        private readonly IMapper _mapper;

        public AppointMentService(
            IMapper mapper,
            IBaseRepository<Appointment> repository,
            IBaseRepository<MedicalRecord> medicalRecordRepository,
            IBaseRepository<Symptom> symptomRepository,
            IBaseRepository<VitalSign> vitalSignRepository,
            IBaseRepository<MedicalDocument> medicalDocumentRepository,
            IAuditLogService auditLogService,
            IBaseRepository<AiAnalysis> aiAnalysisRepository,
            IPatientRepository patientRepository,
            IBaseAccountService accountService) : base(mapper, repository)
        {
            _appointmentRepository = repository;
            _medicalRecordRepository = medicalRecordRepository;
            _symptomRepository = symptomRepository;
            _vitalSignRepository = vitalSignRepository;
            _medicalDocumentRepository = medicalDocumentRepository;
            _aiAnalysisRepository = aiAnalysisRepository;
            _patientRepository = patientRepository;
            _accountService = accountService;
            _auditLogService = auditLogService;
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

        public async Task<List<AppointmentDto>> GetByPatientId(int patientId)
        {
            var all = await _appointmentRepository.GetAllListAsync();
            return _mapper.Map<List<AppointmentDto>>(all.Where(a => a.PatientId == patientId).ToList());
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

        public async Task<AppointmentConsultDto?> GetConsultDetail(int appointmentId)
        {
            var appointment = await _appointmentRepository.GetEntityByIdAsync(appointmentId);
            if (appointment == null) return null;

            var allSymptoms = await _symptomRepository.GetAllListAsync();
            var allVitalSigns = await _vitalSignRepository.GetAllListAsync();
            var allRecords = await _medicalRecordRepository.GetAllListAsync();
            var allDocuments = await _medicalDocumentRepository.GetAllListAsync();

            return new AppointmentConsultDto
            {
                Id = appointment.Id,
                AppointmentDate = appointment.AppointmentDate,
                DoctorId = appointment.DoctorId,
                Reason = appointment.Reason,
                Status = appointment.Status,
                Symptoms = _mapper.Map<List<SymptomDto>>(
                    allSymptoms.Where(s => s.AppointmentId == appointmentId).ToList()),
                VitalSigns = _mapper.Map<List<VitalSignDto>>(
                    allVitalSigns.Where(v => v.AppointmentId == appointmentId).ToList()),
                MedicalRecords = _mapper.Map<List<MedicalRecordDto>>(
                    allRecords.Where(r => r.AppointmentId == appointmentId).ToList()),
                Documents = _mapper.Map<List<MedicalDocumentDto>>(
                    allDocuments.Where(d => d.AppointmentId == appointmentId).ToList())
            };
        }

        public async Task<AppointmentDto?> CreateAppointment(AppointmentDto dto, string currentUserId)
        {
            var result = await SaveDtoAsync(dto); 

            if (result != null)
            {
                await _auditLogService.LogAsync(currentUserId, "CreateAppointment", "Appointment", result.Id.ToString());
            }

            return result;
        }

        public async Task<AppointmentDto?> UpdateAppointment(AppointmentDto dto, int id, string currentUserId)
        {
            var result = await UpdateDtoAsync(dto, id); 

            if (result != null)
            {
                await _auditLogService.LogAsync(currentUserId, "UpdateAppointment", "Appointment", result.Id.ToString());
            }

            return result;
        }

        public async Task<AppointmentDto?> ChangeStatusAsync(int id, string status, string currentUserId)
        {
            if (!Enum.TryParse<AppointmentStatus>(status, true, out var parsedStatus))
                throw new DomainValidationException("Estado no válido. Use: Pending, InProgress, Completed, Cancelled.");

            var appointment = await _appointmentRepository.GetEntityByIdAsync(id);
            if (appointment == null) return null;

            appointment.Status = status;
            var updated = await _appointmentRepository.UpdateEntityAsync(id, appointment);
            if (updated == null) return null;

            var action = parsedStatus switch
            {
                AppointmentStatus.Cancelled => "CancelAppointment",
                AppointmentStatus.Completed => "CompleteAppointment",
                _ => null
            };

            if (action != null)
            {
                await _auditLogService.LogAsync(currentUserId, action, "Appointment", id.ToString());
            }

            return _mapper.Map<AppointmentDto>(updated);
        }

        public async Task<List<AppointmentListItemDto>> GetAllByDoctorAsync(string doctorId)
        {
            var all = await _appointmentRepository.GetAllListAsync();
            var filtered = all.Where(a => a.DoctorId == doctorId)
                              .OrderByDescending(a => a.AppointmentDate)
                              .ToList();
            return await EnrichAppointmentListAsync(filtered);
        }

        public async Task<List<AppointmentListItemDto>> GetFilteredAsync(string doctorId, int? patientId, string? status)
        {
            var all = await _appointmentRepository.GetAllListAsync();
            var query = all.Where(a => a.DoctorId == doctorId);
            if (patientId.HasValue) query = query.Where(a => a.PatientId == patientId.Value);
            if (!string.IsNullOrWhiteSpace(status)) query = query.Where(a => a.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            return await EnrichAppointmentListAsync(query.OrderByDescending(a => a.AppointmentDate).ToList());
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            var entity = await _appointmentRepository.GetEntityByIdAsync(id);
            if (entity == null) return false;
            try
            {
                await _appointmentRepository.RemoveAsync(id);
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        private async Task<List<AppointmentListItemDto>> EnrichAppointmentListAsync(List<Appointment> appointments)
        {
            // Resolver nombres de doctores únicos en batch
            var doctorIds = appointments.Select(a => a.DoctorId).Distinct().ToList();
            var doctorNames = new Dictionary<string, string>();
            foreach (var id in doctorIds)
            {
                var user = await _accountService.GetUserById(id);
                if (user != null) doctorNames[id] = $"{user.Name} {user.LastName}";
            }

            // Resolver nombres de pacientes únicos en batch
            var patientIds = appointments.Select(a => a.PatientId).Distinct().ToList();
            var patients = await _patientRepository.GetAllListAsync();
            var relevantPatients = patients.Where(p => patientIds.Contains(p.Id)).ToList();
            var patientNames = new Dictionary<int, string>();
            foreach (var patient in relevantPatients)
            {
                var user = await _accountService.GetUserById(patient.UserId);
                if (user != null) patientNames[patient.Id] = $"{user.Name} {user.LastName}";
            }

            return appointments.Select(a => new AppointmentListItemDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = patientNames.GetValueOrDefault(a.PatientId, $"Paciente #{a.PatientId}"),
                DoctorId = a.DoctorId,
                DoctorName = doctorNames.GetValueOrDefault(a.DoctorId, a.DoctorId),
                AppointmentDate = a.AppointmentDate,
                Status = a.Status,
                Reason = a.Reason,
                Notes = a.Notes
            }).ToList();
        }

    }
}
