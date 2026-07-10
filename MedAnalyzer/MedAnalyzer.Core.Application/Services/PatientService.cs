using AutoMapper;
using MedAnalyzer.Core.Application.Base;
using MedAnalyzer.Core.Application.Dto.MedicalDocument;
using MedAnalyzer.Core.Application.Dto.MedicalRecord;
using MedAnalyzer.Core.Application.Dto.Patient;
using MedAnalyzer.Core.Application.Dto.User;
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
        private readonly IAccountServiceForWebApi _accountService;
        private readonly IMapper _mapper;

        public PatientService(
            IPatientRepository repository,
            IMedicalRecordRepository medicalRecordRepository,
            IMedicalDocumentRepository medicalDocumentRepository,
            IAppointmentRepository appointmentRepository,
            IAccountServiceForWebApi accountService,
            IMapper mapper) : base(mapper, repository)
        {
            _patientRepository = repository;
            _medicalRecordRepository = medicalRecordRepository;
            _medicalDocumentRepository = medicalDocumentRepository;
            _appointmentRepository = appointmentRepository;
            _accountService = accountService;
            _mapper = mapper;
        }

        public async Task<List<PatientDto>> GetActivePatients()
        {
            var patients = await _patientRepository.GetAllListAsync();
            var dtos = _mapper.Map<List<PatientDto>>(patients.Where(p => p.IsActive).ToList());
            await PopulateUserInfoAsync(dtos);
            return dtos;
        }

        public async Task<List<PatientDto>> SearchPatients(string search)
        {
            var patients = await _patientRepository.GetAllListAsync();
            var filtered = patients.Where(p => p.IsActive &&
                (p.PhoneNumber.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                 p.PatientType.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                 p.UserId.Contains(search, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            var dtos = _mapper.Map<List<PatientDto>>(filtered);
            await PopulateUserInfoAsync(dtos);
            return dtos;
        }

        private async Task PopulateUserInfoAsync(List<PatientDto> dtos)
        {
            var userIds = dtos.Where(d => !string.IsNullOrEmpty(d.UserId)).Select(d => d.UserId).Distinct().ToList();
            if (userIds.Count == 0) return;

            var allUsers = await _accountService.GetAllUser(null);
            var userMap = allUsers.Where(u => !string.IsNullOrEmpty(u.Id)).ToDictionary(u => u.Id);

            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.UserId) && userMap.TryGetValue(dto.UserId, out var user))
                {
                    dto.FullName = $"{user.Name} {user.LastName}";
                    dto.NumberIdentification = user.NumberIdentification;
                    dto.Email = user.Email;
                }
            }
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

            var user = await _accountService.GetUserById(patient.UserId);
            if (user != null)
            {
                detail.FullName = $"{user.Name} {user.LastName}";
                detail.NumberIdentification = user.NumberIdentification;
                detail.Email = user.Email;
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

        public async Task<PatientDto?> UpdatePatientAsync(PatientDto dto, int id)
        {
            var patient = await _patientRepository.GetEntityByIdAsync(id);
            if (patient == null) return null;

            patient.BirthDate = dto.BirthDate;
            patient.Gender = dto.Gender;
            patient.PhoneNumber = dto.PhoneNumber;
            patient.IdentificationType = dto.IdentificationType;
            patient.PatientType = dto.PatientType;
            patient.IsActive = dto.IsActive;
            if (!string.IsNullOrEmpty(dto.UserId))
                patient.UserId = dto.UserId;

            var updated = await _patientRepository.UpdateEntityAsync(id, patient);
            if (updated == null) return null;

            if (!string.IsNullOrEmpty(patient.UserId))
            {
                var user = await _accountService.GetUserById(patient.UserId);
                if (user != null)
                {
                    var updateUserDto = new UpdateUserDto
                    {
                        Name = user.Name,
                        LastName = user.LastName,
                        Email = user.Email,
                        UserName = user.UserName,
                        NumberIdentification = user.NumberIdentification,
                    };

                    bool userChanged = false;

                    if (!string.IsNullOrEmpty(dto.FullName))
                    {
                        var nameParts = dto.FullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                        var newName = nameParts[0];
                        var newLastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

                        if (newName != user.Name || newLastName != user.LastName)
                        {
                            updateUserDto.Name = newName;
                            updateUserDto.LastName = newLastName;
                            userChanged = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(dto.NumberIdentification) && dto.NumberIdentification != user.NumberIdentification)
                    {
                        updateUserDto.NumberIdentification = dto.NumberIdentification;
                        userChanged = true;
                    }

                    if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
                    {
                        updateUserDto.Email = dto.Email;

                        var newUserName = dto.Email.Split('@')[0]
                            .Replace(".", "").Replace("-", "").ToLower();
                        var existing = await _accountService.GetUserByUserName(newUserName);
                        if (existing != null)
                            newUserName += new Random().Next(100, 999).ToString();
                        updateUserDto.UserName = newUserName;
                        userChanged = true;
                    }

                    if (userChanged)
                    {
                        var updateResult = await _accountService.UpdateUserAsync(patient.UserId, updateUserDto);
                        if (updateResult.HasError)
                            return null;
                    }
                }
            }

            return _mapper.Map<PatientDto>(updated);
        }

        public async Task<DesactivatePatient> DeactivatePatient(int id)
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
            return result != null ? DesactivatePatient.Success : DesactivatePatient.Failed;
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
    }
}
