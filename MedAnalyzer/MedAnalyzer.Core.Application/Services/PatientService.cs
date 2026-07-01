using AutoMapper;
using MedAnalyzer.Core.Application.Base;
using MedAnalyzer.Core.Application.Dto.Appointment;
using MedAnalyzer.Core.Application.Dto.MedicalDocument;
using MedAnalyzer.Core.Application.Dto.MedicalRecord;
using MedAnalyzer.Core.Application.Dto.Patient;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Exceptions;
using MedAnalyzer.Core.Domain.Interfaces;

namespace MedAnalyzer.Core.Application.Services
{
    public class PatientService : BaseServices<Patient, PatientDto>, IPatientService
    {
        private readonly IBaseRepository<Patient> _patientRepository;
        private readonly IBaseRepository<MedicalRecord> _medicalRecordRepository;
        private readonly IBaseRepository<MedicalDocument> _medicalDocumentRepository;
        private readonly IBaseRepository<Appointment> _appointmentRepository;
        private readonly IMapper _mapper;

        public PatientService(
            IBaseRepository<Patient> repository,
            IBaseRepository<MedicalRecord> medicalRecordRepository,
            IBaseRepository<MedicalDocument> medicalDocumentRepository,
            IBaseRepository<Appointment> appointmentRepository,
            IMapper mapper) : base(mapper, repository)
        {
            _patientRepository = repository;
            _medicalRecordRepository = medicalRecordRepository;
            _medicalDocumentRepository = medicalDocumentRepository;
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
        }

        public async Task<List<PatientDto>> GetActivePatients()
        {
            var patients = await _patientRepository.GetAllListAsync();
            var active = patients.Where(p => p.IsActive)
                                 .OrderByDescending(p => p.CreatedAt)
                                 .ToList();
            return _mapper.Map<List<PatientDto>>(active);
        }

        public async Task<List<PatientDto>> SearchPatients(string search)
        {
            var patients = await _patientRepository.GetAllListAsync();
            var filtered = patients.Where(p => p.IsActive &&
                (p.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                 p.IdentificationNumber.Contains(search, StringComparison.OrdinalIgnoreCase)))
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

            var allAppointments = await _appointmentRepository.GetAllListAsync();
            var patientAppointments = allAppointments
                .Where(a => a.PatientId == id)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();

            var detail = _mapper.Map<PatientDetailDto>(patient);
            detail.Appointments = _mapper.Map<List<AppointmentSummaryDto>>(patientAppointments);
            detail.MedicalRecords = _mapper.Map<List<MedicalRecordSummaryDto>>(patientRecords);
            detail.Documents = _mapper.Map<List<MedicalDocumentDto>>(patientDocuments);

            return detail;
        }

        public override async Task<PatientDto?> SaveDtoAsync(PatientDto dto)
        {
            Validate(dto);
            try
            {
                return await base.SaveDtoAsync(dto);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override async Task<PatientDto?> UpdateDtoAsync(PatientDto dto, int id)
        {
            Validate(dto);
            return await base.UpdateDtoAsync(dto, id);
        }

        private static readonly string[] ValidGenders = ["Masculino", "Femenino", "Otro"];

        private static void Validate(PatientDto dto)
        {
            if (dto.BirthDate >= DateOnly.FromDateTime(DateTime.Today))
                throw new DomainValidationException("La fecha de nacimiento no puede ser una fecha futura.");
            if (!ValidGenders.Contains(dto.Gender))
                throw new DomainValidationException("Género no válido. Use: Masculino, Femenino u Otro.");
        }

        public async Task<bool> SoftDeletePatient(int id)
        {
            var patient = await _patientRepository.GetEntityByIdAsync(id);
            if (patient == null) return false;

            patient.IsActive = false;
            var result = await _patientRepository.UpdateEntityAsync(id, patient);
            return result != null;
        }
    }
}