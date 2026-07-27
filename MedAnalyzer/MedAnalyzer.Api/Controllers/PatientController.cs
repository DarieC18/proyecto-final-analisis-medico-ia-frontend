using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.Patient;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Gestión de pacientes. Creación y consulta de expedientes clínicos.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IAccountServiceForWebApi _accountService;
        private readonly IEmailService _emailService;

        public PatientController(IPatientService patientService, IAccountServiceForWebApi accountService, IEmailService emailService)
        {
            _patientService = patientService;
            _accountService = accountService;
            _emailService = emailService;
        }

        /// <summary>Obtiene todos los pacientes activos.</summary>
        [HttpGet]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(List<PatientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _patientService.GetActivePatients();
            if (patients == null || patients.Count == 0)
                return NoContent();
            return Ok(patients);
        }

        /// <summary>Busca pacientes por teléfono, tipo o UserId.</summary>
        [HttpGet("search")]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(List<PatientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var patients = await _patientService.SearchPatients(query);
            if (patients == null || patients.Count == 0)
                return NoContent();
            return Ok(patients);
        }

        /// <summary>Obtiene un paciente por ID.</summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _patientService.GetDtoById(id);
            if (patient == null)
                return NotFound(new ErrorResponse { Message = "Paciente no encontrado." });

            if (!string.IsNullOrWhiteSpace(patient.UserId))
            {
                var user = await _accountService.GetUserById(patient.UserId);
                if (user != null)
                    patient.FullName = $"{user.Name} {user.LastName}";
            }

            return Ok(patient);
        }

        /// <summary>Obtiene el detalle completo del paciente incluyendo historial y documentos.</summary>
        [HttpGet("{id}/details")]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(PatientDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetails(int id)
        {
            var detail = await _patientService.GetPatientDetail(id);
            if (detail == null)
                return NotFound(new ErrorResponse { Message = "Paciente no encontrado." });

            if (!string.IsNullOrWhiteSpace(detail.UserId))
            {
                var user = await _accountService.GetUserById(detail.UserId);
                if (user != null)
                    detail.FullName = $"{user.Name} {user.LastName}";
            }

            return Ok(detail);
        }

        /// <summary>El doctor crea un paciente. Se crea automáticamente su cuenta de portal y se envían credenciales por correo.</summary>
        [HttpPost("create")]
        [Authorize(Roles = "Doctor")]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePatientByDoctorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string userId;
            string resetToken;
            try
            {
                (userId, resetToken) = await _accountService.RegisterPatientAccountAsync(
                    dto.Email, dto.FirstName, dto.LastName, dto.NumberIdentification);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Message = ex.Message });
            }

            var patientDto = new PatientDto
            {
                Id = 0,
                UserId = userId,
                PhoneNumber = dto.PhoneNumber,
                Gender = dto.Gender,
                BirthDate = dto.BirthDate,
                IdentificationType = dto.IdentificationType,
                PatientType = dto.PatientType,
                IsActive = true
            };

            var result = await _patientService.SaveDtoAsync(patientDto);
            if (result == null)
            {
                await _accountService.DeleteAsync(userId);
                return BadRequest(new ErrorResponse { Message = "Error al crear el expediente del paciente." });
            }

            await _emailService.SendPatientActivationEmailAsync(dto.Email, dto.FirstName, userId, resetToken);

            return StatusCode(201, result);
        }

        /// <summary>Actualiza los datos clínicos de un paciente.</summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] PatientDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _patientService.UpdatePatientAsync(dto, id);
            if (result == null)
                return BadRequest(new ErrorResponse { Message = "Error al actualizar el paciente." });

            var result = await _sender.Send(new UpdatePatientCommand(dto, id, currentUser));
            if (result == null) return BadRequest(new ErrorResponse { Message = "Error al actualizar el paciente." });
            return Ok(result);
        }

        /// <summary>Desactiva el expediente clínico de un paciente.</summary>
        [HttpPatch("{id}/deactivate")]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _patientService.DeactivatePatient(id);

            if (result == DesactivatePatient.NotFound)
                return NotFound(new ErrorResponse { Message = "Paciente no encontrado." });
            if (result == DesactivatePatient.HasActiveAppointments)
                return BadRequest(new ErrorResponse { Message = "El paciente tiene citas activas." });
            if (result == DesactivatePatient.Failed)
                return BadRequest(new ErrorResponse { Message = "Error al desactivar el paciente." });

            return Ok(new MessageResponse { Message = "Paciente desactivado exitosamente." });
        }

        /// <summary>Elimina permanentemente un paciente.</summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _patientService.DeletePatient(id);
            if (!result)
                return BadRequest(new ErrorResponse { Message = "No se puede eliminar el paciente. Verifique que no tenga citas o documentos asociados." });

            return NoContent();
        }
    }
}
