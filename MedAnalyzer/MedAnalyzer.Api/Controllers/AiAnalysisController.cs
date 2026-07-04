using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.AiAnalisys;
using MedAnalyzer.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Consulta de análisis generados por inteligencia artificial.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Nurse")]
    public class AiAnalysisController : ControllerBase
    {
        private readonly IAiAnalisysServices _aiService;

        public AiAnalysisController(IAiAnalisysServices aiService)
        {
            _aiService = aiService;
        }

        /// <summary>Obtiene los análisis de IA asociados a una cita médica.</summary>
        /// <param name="appointmentId">Identificador de la cita.</param>
        /// <returns>Lista de análisis de IA de la cita.</returns>
        [HttpGet("by-appointment/{appointmentId}")]
        [ProducesResponseType(typeof(List<AiAnalisysDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
        {
            var list = await _aiService.GetByAppointmentIdAsync(appointmentId);
            return Ok(list);
        }

        /// <summary>Obtiene todos los análisis de IA de un paciente.</summary>
        /// <param name="patientId">Identificador del paciente.</param>
        /// <returns>Lista de análisis de IA del paciente.</returns>
        [HttpGet("by-patient/{patientId}")]
        [ProducesResponseType(typeof(List<AiAnalisysDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            var list = await _aiService.GetByPatientIdAsync(patientId);
            return Ok(list);
        }

        /// <summary>Obtiene un análisis de IA por su identificador.</summary>
        /// <param name="id">Identificador del análisis.</param>
        /// <returns>Datos del análisis de IA.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AiAnalisysDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var analysis = await _aiService.GetDtoById(id);
            if (analysis == null)
                return NotFound(new ErrorResponse { Message = "Análisis de IA no encontrado." });
            return Ok(analysis);
        }

        /// <summary>Marca un análisis de IA como revisado por el médico.</summary>
        /// <param name="id">Identificador del análisis.</param>
        /// <returns>Sin contenido si la operación fue exitosa.</returns>
        [HttpPatch("{id}/review")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkReviewed(int id)
        {
            var dto = await _aiService.GetDtoById(id);
            if (dto == null)
                return NotFound(new ErrorResponse { Message = "Análisis de IA no encontrado." });

            dto.IsReviewed = true;
            await _aiService.UpdateDtoAsync(dto, id);
            return NoContent();
        }
    }
}
