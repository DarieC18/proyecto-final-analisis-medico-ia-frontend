using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.MedicalRecord;
using MedAnalyzer.Core.Application.Features.MedicalRecords.Commands;
using MedAnalyzer.Core.Application.Features.MedicalRecords.Queries;
using MediatR;
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
        private readonly ISender _sender;
        public MedicalRecordController(ISender sender) => _sender = sender;

        [HttpGet("by-appointment/{appointmentId}")]
        [ProducesResponseType(typeof(List<MedicalRecordDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
            => Ok(await _sender.Send(new GetMedicalRecordsByAppointmentQuery(appointmentId)));

        [HttpGet("by-patient/{patientId}")]
        [ProducesResponseType(typeof(List<MedicalRecordDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPatient(int patientId)
            => Ok(await _sender.Send(new GetMedicalRecordsByPatientQuery(patientId)));

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MedicalRecordDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _sender.Send(new GetMedicalRecordByIdQuery(id));
            if (result == null) return NotFound(new ErrorResponse { Message = "Registro clínico no encontrado." });
            return Ok(result);
        }

        /// <summary>Crea un nuevo registro clínico asociado a una cita.</summary>
        /// <param name="dto">Datos del registro clínico.</param>
        /// <returns>Datos del registro clínico creado.</returns>
        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ProducesResponseType(typeof(MedicalRecordDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] MedicalRecordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto.PatientId == 0) return BadRequest(new ErrorResponse { Message = "El paciente es requerido." });
            if (dto.AppointmentId == 0) return BadRequest(new ErrorResponse { Message = "La cita asociada es requerida." });

            var uid = User.FindFirstValue("uid") ?? "";
            var result = await _sender.Send(new CreateMedicalRecordCommand(dto, uid));
            if (result == null) return BadRequest(new ErrorResponse { Message = "Error al guardar el historial clínico." });
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MedicalRecordDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] MedicalRecordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto.PatientId == 0) return BadRequest(new ErrorResponse { Message = "El paciente es requerido." });
            if (dto.AppointmentId == 0) return BadRequest(new ErrorResponse { Message = "La cita asociada es requerida." });

            var uid = User.FindFirstValue("uid") ?? "";
            var result = await _sender.Send(new UpdateMedicalRecordCommand(dto, id, uid));
            if (result == null) return NotFound(new ErrorResponse { Message = "Registro clínico no encontrado." });
            return Ok(result);
        }

        [HttpGet("detail-by-patient/{patientId}")]
        [ProducesResponseType(typeof(List<MedicalRecordSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDetailsByPatient(int patientId)
        {
            var result = await _sender.Send(new GetMedicalRecordSummariesByPatientQuery(patientId));
            if (result == null) return NotFound(new ErrorResponse { Message = "El paciente no tiene registros clínicos." });
            return Ok(result);
        }
    }
}
