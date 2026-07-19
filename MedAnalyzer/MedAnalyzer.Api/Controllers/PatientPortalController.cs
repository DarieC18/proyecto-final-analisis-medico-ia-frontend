using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.Appointment;
using MedAnalyzer.Core.Application.Dto.Patient;
using MedAnalyzer.Core.Application.Dto.User;
using MedAnalyzer.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Portal del paciente. Todos los endpoints requieren rol Patient.</summary>
    [Route("api/v1/portal")]
    [ApiController]
    [Authorize(Roles = "Patient")]
    public class PatientPortalController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IAccountServiceForWebApi _accountService;
        private readonly IAppointmentService _appointmentService;
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IRecommendationService _recommendationService;
        private readonly IAiAnalisysServices _aiService;
        private readonly IMedicalDocumentService _documentService;

        public PatientPortalController(
            IPatientService patientService,
            IAccountServiceForWebApi accountService,
            IAppointmentService appointmentService,
            IMedicalRecordService medicalRecordService,
            IRecommendationService recommendationService,
            IAiAnalisysServices aiService,
            IMedicalDocumentService documentService)
        {
            _patientService = patientService;
            _accountService = accountService;
            _appointmentService = appointmentService;
            _medicalRecordService = medicalRecordService;
            _recommendationService = recommendationService;
            _aiService = aiService;
            _documentService = documentService;
        }

        private async Task<(PatientDto? patient, string uid)> GetLinkedPatient()
        {
            var uid = User.FindFirstValue("uid") ?? string.Empty;
            var patient = await _patientService.GetByUserId(uid);
            return (patient, uid);
        }

        /// <summary>Devuelve el perfil combinado del paciente (datos de cuenta + datos clínicos).</summary>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(PatientPortalProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile()
        {
            var (patient, uid) = await GetLinkedPatient();
            if (patient == null)
                return NotFound(new ErrorResponse { Message = "No tienes un expediente clínico vinculado al sistema." });

            var user = await _accountService.GetUserById(uid);
            if (user == null)
                return NotFound(new ErrorResponse { Message = "Usuario no encontrado." });

            return Ok(new PatientPortalProfileDto
            {
                PatientId = patient.Id,
                UserId = uid,
                FullName = $"{user.Name} {user.LastName}",
                Email = user.Email,
                UserName = user.UserName,
                NumberIdentification = user.NumberIdentification,
                BirthDate = patient.BirthDate,
                Gender = patient.Gender,
                PhoneNumber = patient.PhoneNumber,
                IdentificationType = patient.IdentificationType,
                PatientType = patient.PatientType,
                IsActive = patient.IsActive
            });
        }

        /// <summary>Actualiza los datos clínicos del perfil del paciente (fecha de nacimiento, género, teléfono, etc.).</summary>
        [HttpPatch("profile")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdatePatientProfileDto dto)
        {
            var (patient, _) = await GetLinkedPatient();
            if (patient == null)
                return NotFound(new ErrorResponse { Message = "No tienes un expediente clínico vinculado al sistema." });

            patient.BirthDate = dto.BirthDate;
            patient.Gender = dto.Gender;
            patient.PhoneNumber = dto.PhoneNumber;
            patient.IdentificationType = dto.IdentificationType;
            patient.PatientType = dto.PatientType;

            await _patientService.UpdateDtoAsync(patient, patient.Id);

            return Ok(new MessageResponse { Message = "Perfil actualizado exitosamente." });
        }

        /// <summary>Lista los médicos disponibles por especialidad para seleccionar al solicitar cita.</summary>
        [HttpGet("doctors")]
        [ProducesResponseType(typeof(List<DoctorListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctors()
        {
            var allUsers = await _accountService.GetAllUser(true);
            var doctors = allUsers
                .Where(u => u.Role == "Doctor")
                .Select(u => new DoctorListItemDto
                {
                    Id = u.Id,
                    FullName = $"{u.Name} {u.LastName}",
                    Specialty = u.Specialty
                })
                .ToList();

            return Ok(doctors);
        }

        /// <summary>Lista todas las citas del paciente autenticado.</summary>
        [HttpGet("appointments")]
        [ProducesResponseType(typeof(List<AppointmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAppointments()
        {
            var (patient, _) = await GetLinkedPatient();
            if (patient == null)
                return NotFound(new ErrorResponse { Message = "No tienes un expediente clínico vinculado al sistema." });

            var appointments = await _appointmentService.GetByPatientId(patient.Id);
            return Ok(appointments);
        }

        /// <summary>Solicita una nueva cita. Si no se especifica médico, se asigna uno de medicina general.</summary>
        [HttpPost("appointments")]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestAppointment([FromBody] PatientAppointmentRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (patient, _) = await GetLinkedPatient();
            if (patient == null)
                return NotFound(new ErrorResponse { Message = "No tienes un expediente clínico vinculado al sistema." });

            var doctorId = dto.DoctorId;
            if (string.IsNullOrWhiteSpace(doctorId))
            {
                var allUsers = await _accountService.GetAllUser(true);
                var general = allUsers
                    .Where(u => u.Role == "Doctor" && (string.IsNullOrWhiteSpace(u.Specialty) || u.Specialty == "General"))
                    .FirstOrDefault();

                if (general == null)
                    return BadRequest(new ErrorResponse { Message = "No hay médicos generales disponibles. Por favor selecciona un médico." });

                doctorId = general.Id;
            }

            var result = await _appointmentService.SaveDtoAsync(new AppointmentDto
            {
                Id = 0,
                PatientId = patient.Id,
                DoctorId = doctorId,
                AppointmentDate = dto.AppointmentDate,
                Reason = dto.Reason,
                Notes = dto.Notes,
                Status = "Pending"
            });

            if (result == null)
                return BadRequest(new ErrorResponse { Message = "Error al crear la cita." });

            return StatusCode(201, result);
        }

        /// <summary>Cancela una cita. Solo se puede cancelar si está en estado Pending.</summary>
        [HttpPatch("appointments/{id}/cancel")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var (patient, _) = await GetLinkedPatient();
            if (patient == null)
                return NotFound(new ErrorResponse { Message = "No tienes un expediente clínico vinculado al sistema." });

            var appointment = await _appointmentService.GetDtoById(id);
            if (appointment == null)
                return NotFound(new ErrorResponse { Message = "Cita no encontrada." });

            if (appointment.PatientId != patient.Id)
                return StatusCode(403, new ErrorResponse { Message = "No tienes permiso para cancelar esta cita." });

            if (appointment.Status != "Pending")
                return BadRequest(new ErrorResponse { Message = $"No se puede cancelar una cita en estado '{appointment.Status}'. Solo se pueden cancelar citas en estado Pending." });

            await _appointmentService.ChangeStatusAsync(id, "Cancelled");

            return Ok(new MessageResponse { Message = "Cita cancelada exitosamente." });
        }

        /// <summary>Historial clínico del paciente.</summary>
        [HttpGet("medical-records")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMedicalRecords()
        {
            var (patient, _) = await GetLinkedPatient();
            if (patient == null)
                return NotFound(new ErrorResponse { Message = "No tienes un expediente clínico vinculado al sistema." });

            var records = await _medicalRecordService.GetByPatientId(patient.Id);
            return Ok(records);
        }

        /// <summary>Indicaciones y recetas del médico.</summary>
        [HttpGet("recommendations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRecommendations()
        {
            var (patient, _) = await GetLinkedPatient();
            if (patient == null)
                return NotFound(new ErrorResponse { Message = "No tienes un expediente clínico vinculado al sistema." });

            var recs = await _recommendationService.GetByPatientId(patient.Id);
            return Ok(recs);
        }

        /// <summary>Resultados de análisis de IA del paciente.</summary>
        [HttpGet("results")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetResults()
        {
            var (patient, _) = await GetLinkedPatient();
            if (patient == null)
                return NotFound(new ErrorResponse { Message = "No tienes un expediente clínico vinculado al sistema." });

            var results = await _aiService.GetByPatientIdAsync(patient.Id);
            return Ok(results);
        }

        /// <summary>Documentos médicos del paciente.</summary>
        [HttpGet("documents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDocuments()
        {
            var (patient, _) = await GetLinkedPatient();
            if (patient == null)
                return NotFound(new ErrorResponse { Message = "No tienes un expediente clínico vinculado al sistema." });

            var docs = await _documentService.GetByPatientId(patient.Id);
            return Ok(docs);
        }
    }
}
