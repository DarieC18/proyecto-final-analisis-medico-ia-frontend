using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.Alert;
using MedAnalyzer.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Gestión de alertas clínicas generadas por el sistema.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Nurse")]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        /// <summary>Obtiene las alertas asociadas a un paciente.</summary>
        /// <param name="patientId">Identificador del paciente.</param>
        /// <returns>Lista de alertas del paciente.</returns>
        [HttpGet("by-patient/{patientId}")]
        [ProducesResponseType(typeof(List<AlertDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            var alerts = await _alertService.GetByPatientIdAsync(patientId);
            return Ok(alerts);
        }

        /// <summary>Obtiene las alertas asociadas a una cita médica.</summary>
        /// <param name="appointmentId">Identificador de la cita.</param>
        /// <returns>Lista de alertas de la cita.</returns>
        [HttpGet("by-appointment/{appointmentId}")]
        [ProducesResponseType(typeof(List<AlertDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
        {
            var alerts = await _alertService.GetByAppointmentIdAsync(appointmentId);
            return Ok(alerts);
        }

        /// <summary>Obtiene todas las alertas que aún no han sido resueltas.</summary>
        /// <returns>Lista de alertas activas.</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(List<AlertDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActive()
        {
            var active = await _alertService.GetActiveAsync();
            return Ok(active);
        }

        /// <summary>Marca una alerta como resuelta.</summary>
        /// <param name="id">Identificador de la alerta.</param>
        /// <returns>Sin contenido si la operación fue exitosa.</returns>
        [HttpPatch("{id}/resolve")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Resolve(int id)
        {
            var dto = await _alertService.GetDtoById(id);
            if (dto == null)
                return NotFound(new ErrorResponse { Message = "Alerta no encontrada." });

            dto.IsResolved = true;
            await _alertService.UpdateDtoAsync(dto, id);
            return NoContent();
        }
    }
}
