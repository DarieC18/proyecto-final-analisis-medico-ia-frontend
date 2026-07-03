using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.MedicalRecord;
using MedAnalyzer.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Gestión de registros clínicos asociados a citas y pacientes.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Nurse")]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        /// <summary>Obtiene los registros clínicos de una cita médica.</summary>
        /// <param name="appointmentId">Identificador de la cita.</param>
        /// <returns>Lista de registros clínicos de la cita.</returns>
        [HttpGet("by-appointment/{appointmentId}")]
        [ProducesResponseType(typeof(List<MedicalRecordDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
        {
            var records = await _medicalRecordService.GetByAppointmentId(appointmentId);
            return Ok(records ?? []);
        }

        /// <summary>Obtiene todos los registros clínicos de un paciente.</summary>
        /// <param name="patientId">Identificador del paciente.</param>
        /// <returns>Lista de registros clínicos del paciente.</returns>
        [HttpGet("by-patient/{patientId}")]
        [ProducesResponseType(typeof(List<MedicalRecordDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            var records = await _medicalRecordService.GetByPatientId(patientId);
            return Ok(records ?? []);
        }

        /// <summary>Obtiene un registro clínico por su identificador.</summary>
        /// <param name="id">Identificador del registro.</param>
        /// <returns>Datos del registro clínico.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MedicalRecordDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _medicalRecordService.GetDtoById(id);
            if (record == null)
                return NotFound(new ErrorResponse { Message = "Registro clínico no encontrado." });
            return Ok(record);
        }

        /// <summary>Crea un nuevo registro clínico asociado a una cita.</summary>
        /// <param name="dto">Datos del registro clínico.</param>
        /// <returns>Datos del registro clínico creado.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(MedicalRecordDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] MedicalRecordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.PatientId == 0)
                return BadRequest(new ErrorResponse { Message = "El paciente es requerido." });

            if (dto.AppointmentId == 0)
                return BadRequest(new ErrorResponse { Message = "La cita asociada es requerida." });

            var currentUserId = User.FindFirstValue("uid") ?? "";
            if (string.IsNullOrWhiteSpace(dto.CreatedByUserId))
                dto.CreatedByUserId = currentUserId;

            var result = await _medicalRecordService.SaveDtoAsync(dto);
            if (result == null)
                return BadRequest(new ErrorResponse { Message = "Error al guardar el historial clínico." });

            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>Actualiza un registro clínico existente.</summary>
        /// <param name="id">Identificador del registro.</param>
        /// <param name="dto">Nuevos datos del registro clínico.</param>
        /// <returns>Datos del registro clínico actualizado.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MedicalRecordDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] MedicalRecordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.PatientId == 0)
                return BadRequest(new ErrorResponse { Message = "El paciente es requerido." });

            if (dto.AppointmentId == 0)
                return BadRequest(new ErrorResponse { Message = "La cita asociada es requerida." });

            var result = await _medicalRecordService.UpdateDtoAsync(dto, id);
            if (result == null)
                return NotFound(new ErrorResponse { Message = "Registro clínico no encontrado." });

            return Ok(result);
        }
    }
}
