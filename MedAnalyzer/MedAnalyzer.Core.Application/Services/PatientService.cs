using AutoMapper;
using MedAnalyzer.Core.Application.Base;
using MedAnalyzer.Core.Application.Dto.MedicalDocument;
using MedAnalyzer.Core.Application.Dto.MedicalRecord;
using MedAnalyzer.Core.Application.Dto.Patient;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Enum;
using MedAnalyzer.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedAnalyzer.Core.Application.Services
{
    public class PatientService : BaseServices<Patient, PatientDto>, IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IMedicalDocumentRepository _medicalDocumentRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAuditLogService _auditLogService;
        private readonly IBaseAccountService _accountService;
        private readonly IMapper _mapper;

        public PatientService(
            IPatientRepository repository,
            IMedicalRecordRepository medicalRecordRepository,
            IMedicalDocumentRepository medicalDocumentRepository,
            IAppointmentRepository appointmentRepository,
            IAuditLogService auditLogService,
            IBaseAccountService accountService,
            IMapper mapper) : base(mapper, repository)
        {
            _patientRepository = repository;
            _medicalRecordRepository = medicalRecordRepository;
            _medicalDocumentRepository = medicalDocumentRepository;
            _appointmentRepository = appointmentRepository;
            _auditLogService = auditLogService;
            _accountService = accountService;
            _mapper = mapper;
        }

        public async Task<List<PatientDto>> GetActivePatients()
        {
            var patients = await _patientRepository.GetAllListAsync();
            return _mapper.Map<List<PatientDto>>(patients.Where(p => p.IsActive).ToList());
        }

        public async Task<List<PatientDto>> SearchPatients(string search)
        {
            var patients = await _patientRepository.GetAllListAsync();
            var filtered = patients.Where(p => p.IsActive &&
                (p.PhoneNumber.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                 p.PatientType.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                 p.UserId.Contains(search, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            return _mapper.Map<List<PatientDto>>(filtered);
        }

        public async Task<PatientDetailDto?> GetPatientDetail(int id)
        {
            var patient = await _patientRepository.GetEntityByIdAsync(id);
            if (patient == null) return null;

            var allRecords = await _medicalRecordRepository.GetAllListWithInclude(["Appointment"]);
            var patientRecords = allRecords.Where(r => r.PatientId == id).ToList();

            var allDocuments = await _medicalDocumentRepository.GetAllListAsync();
            var patientDocuments = allDocuments.Where(d => d.PatientId == id).ToList();

            var detail = _mapper.Map<PatientDetailDto>(patient);
            detail.MedicalRecords = _mapper.Map<List<MedicalRecordSummaryDto>>(patientRecords);
            detail.Documents = _mapper.Map<List<MedicalDocumentDto>>(patientDocuments);

            // DoctorName viene del mapper con el UUID; lo resolvemos a nombre real
            var uniqueDoctorIds = detail.MedicalRecords
                .Where(r => !string.IsNullOrEmpty(r.DoctorName))
                .Select(r => r.DoctorName)
                .Distinct()
                .ToList();

            var doctorNames = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var doctorId in uniqueDoctorIds)
            {
                var user = await _accountService.GetUserById(doctorId);
                if (user != null)
                    doctorNames[doctorId] = $"{user.Name} {user.LastName}";
            }

            foreach (var record in detail.MedicalRecords)
            {
                if (doctorNames.TryGetValue(record.DoctorName, out var name))
                    record.DoctorName = name;
            }

            return detail;
        }

        public async Task<PatientDto?> GetByUserId(string userId)
        {
            var patients = await _patientRepository.GetAllListAsync();
            var patient = patients.FirstOrDefault(p => p.UserId == userId);
            return patient == null ? null : _mapper.Map<PatientDto>(patient);
        }

        public async Task LinkUserAsync(int patientId, string userId)
        {
            var patient = await _patientRepository.GetEntityByIdAsync(patientId);
            if (patient == null) return;
            patient.UserId = userId;
            await _patientRepository.UpdateEntityAsync(patientId, patient);
        }

        public async Task<DesactivatePatient> DeactivatePatient(int id, string currentUserId)
        {
            var patient = await _patientRepository.GetEntityByIdAsync(id);
            if (patient == null) return DesactivatePatient.NotFound;

            var appoinments = await _appointmentRepository.GetAllQuery().Where(a => a.PatientId == id && a.Status != AppointmentStatus.Completed.ToString() 
            && a.Status != AppointmentStatus.Cancelled.ToString()).ToListAsync();

            if (appoinments.Any())
            {
                return DesactivatePatient.HasActiveAppointments;
            }

            patient.IsActive = false;

            var result = await _patientRepository.UpdateEntityAsync(id, patient);


            if (result != null)
            {
               await _auditLogService.LogAsync(currentUserId, "DeactivatePatient", "Patient", result.Id.ToString());
            }


            return result != null ? DesactivatePatient.Success : DesactivatePatient.Failed;
        }

        public async Task<PatientDto?> CreatePatient(PatientDto patient, string currentUserId)
        {
            var result = await base.SaveDtoAsync(patient);

            if(result != null)
            {
                await _auditLogService.LogAsync(currentUserId, "CreatePatient", "Patient", result.Id.ToString());
            }

            return result;
        }

        public async Task<bool> DeletePatient(int id)
        {
            var patient = await _patientRepository.GetEntityByIdAsync(id);
            if (patient == null) return false;

            try
            {
                await _patientRepository.RemoveAsync(id);
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<PatientDto?> UpdatePatient(PatientDto patient, int id, string currentUserId)
        {
            var result = await base.UpdateDtoAsync(patient, id);

            if (result != null) 
            {
                await _auditLogService.LogAsync(currentUserId, "UpdatePatient", "Patient", result.Id.ToString());
            }

            return result;
        }
    }
}
