using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.VitalSign;
using MedAnalyzer.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Gestión de signos vitales registrados durante una cita médica.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Nurse")]
    public class VitalSignController : ControllerBase
    {
        private readonly IVitalSignService _vitalSignService;

        public VitalSignController(IVitalSignService vitalSignService)
        {
            _vitalSignService = vitalSignService;
        }

        /// <summary>Obtiene los signos vitales registrados en una cita médica.</summary>
        /// <param name="appointmentId">Identificador de la cita.</param>
        /// <returns>Lista de registros de signos vitales de la cita.</returns>
        [HttpGet("by-appointment/{appointmentId}")]
        [ProducesResponseType(typeof(List<VitalSignDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
        {
            var signs = await _vitalSignService.GetByAppointmentId(appointmentId);
            return Ok(signs ?? []);
        }

        /// <summary>Obtiene un registro de signos vitales por su identificador.</summary>
        /// <param name="id">Identificador del registro.</param>
        /// <returns>Datos de los signos vitales.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VitalSignDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var sign = await _vitalSignService.GetDtoById(id);
            if (sign == null)
                return NotFound(new ErrorResponse { Message = "Signos vitales no encontrados." });
            return Ok(sign);
        }

        /// <summary>
        /// Registra signos vitales en una cita y genera alertas automáticas para valores fuera de rango.
        /// Temperatura: 30–45 °C. Saturación de oxígeno: 0–100%.
        /// </summary>
        /// <param name="dto">Datos de los signos vitales a registrar.</param>
        /// <returns>Datos de los signos vitales registrados.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(VitalSignDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] VitalSignDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.MeasuredAt == default)
                dto.MeasuredAt = DateTime.UtcNow;

            var result = await _vitalSignService.RegisterWithAlerts(dto);
            if (result == null)
                return BadRequest(new ErrorResponse { Message = "Error al registrar los signos vitales." });

            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>Actualiza un registro de signos vitales existente.</summary>
        /// <param name="id">Identificador del registro.</param>
        /// <param name="dto">Nuevos datos de los signos vitales.</param>
        /// <returns>Datos del registro actualizado.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(VitalSignDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] VitalSignDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _vitalSignService.UpdateDtoAsync(dto, id);
            if (result == null)
                return NotFound(new ErrorResponse { Message = "Signos vitales no encontrados." });

            return Ok(result);
        }
    }
}
