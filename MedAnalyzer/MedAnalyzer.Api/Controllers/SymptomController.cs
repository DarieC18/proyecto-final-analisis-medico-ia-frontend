using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.Symptom;
using MedAnalyzer.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Gestión de síntomas registrados durante una cita médica.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Nurse")]
    public class SymptomController : ControllerBase
    {
        private readonly ISymptomService _symptomService;

        public SymptomController(ISymptomService symptomService)
        {
            _symptomService = symptomService;
        }

        /// <summary>Obtiene los síntomas registrados en una cita médica.</summary>
        /// <param name="appointmentId">Identificador de la cita.</param>
        /// <returns>Lista de síntomas de la cita.</returns>
        [HttpGet("by-appointment/{appointmentId}")]
        [ProducesResponseType(typeof(List<SymptomDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
        {
            var symptoms = await _symptomService.GetByAppointmentId(appointmentId);
            return Ok(symptoms ?? []);
        }

        /// <summary>Obtiene un síntoma por su identificador.</summary>
        /// <param name="id">Identificador del síntoma.</param>
        /// <returns>Datos del síntoma.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SymptomDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var symptom = await _symptomService.GetDtoById(id);
            if (symptom == null)
                return NotFound(new ErrorResponse { Message = "Síntoma no encontrado." });
            return Ok(symptom);
        }

        /// <summary>Registra un nuevo síntoma en una cita. Severidad válida: Leve, Moderado, Severo.</summary>
        /// <param name="dto">Datos del síntoma a registrar.</param>
        /// <returns>Datos del síntoma registrado.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(SymptomDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] SymptomDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _symptomService.SaveDtoAsync(dto);
            if (result == null)
                return BadRequest(new ErrorResponse { Message = "Error al registrar el síntoma." });

            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>Actualiza los datos de un síntoma existente.</summary>
        /// <param name="id">Identificador del síntoma.</param>
        /// <param name="dto">Nuevos datos del síntoma.</param>
        /// <returns>Datos del síntoma actualizado.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SymptomDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] SymptomDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _symptomService.UpdateDtoAsync(dto, id);
            if (result == null)
                return NotFound(new ErrorResponse { Message = "Síntoma no encontrado." });

            return Ok(result);
        }

        /// <summary>Elimina permanentemente un síntoma.</summary>
        /// <param name="id">Identificador del síntoma.</param>
        /// <returns>Sin contenido si la operación fue exitosa.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _symptomService.GetDtoById(id);
            if (existing == null)
                return NotFound(new ErrorResponse { Message = "Síntoma no encontrado." });

            await _symptomService.DeleteHardDtoAsync(id);
            return NoContent();
        }
    }
}
