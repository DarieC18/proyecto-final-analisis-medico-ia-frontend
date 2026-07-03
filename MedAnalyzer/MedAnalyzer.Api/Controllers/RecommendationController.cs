using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.Recommendation;
using MedAnalyzer.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Consulta de recomendaciones generadas por el sistema de IA.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Nurse")]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        /// <summary>Obtiene las recomendaciones asociadas a una cita médica.</summary>
        /// <param name="appointmentId">Identificador de la cita.</param>
        /// <returns>Lista de recomendaciones de la cita.</returns>
        [HttpGet("by-appointment/{appointmentId}")]
        [ProducesResponseType(typeof(List<RecommendationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
        {
            var list = await _recommendationService.GetByAppointmentId(appointmentId);
            return Ok(list);
        }

        /// <summary>Obtiene todas las recomendaciones de un paciente.</summary>
        /// <param name="patientId">Identificador del paciente.</param>
        /// <returns>Lista de recomendaciones del paciente.</returns>
        [HttpGet("by-patient/{patientId}")]
        [ProducesResponseType(typeof(List<RecommendationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            var list = await _recommendationService.GetByPatientId(patientId);
            return Ok(list);
        }

        /// <summary>Obtiene una recomendación por su identificador.</summary>
        /// <param name="id">Identificador de la recomendación.</param>
        /// <returns>Datos de la recomendación.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RecommendationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var rec = await _recommendationService.GetDtoById(id);
            if (rec == null)
                return NotFound(new ErrorResponse { Message = "Recomendación no encontrada." });
            return Ok(rec);
        }
    }
}
