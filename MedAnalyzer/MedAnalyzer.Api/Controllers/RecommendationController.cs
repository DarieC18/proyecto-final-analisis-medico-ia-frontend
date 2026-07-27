using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.Recommendation;
using MedAnalyzer.Core.Application.Features.Recommendations.Queries;
using MediatR;
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
        private readonly ISender _sender;
        public RecommendationController(ISender sender) => _sender = sender;

        [HttpGet("by-appointment/{appointmentId}")]
        [ProducesResponseType(typeof(List<RecommendationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
            => Ok(await _sender.Send(new GetRecommendationsByAppointmentQuery(appointmentId)));

        [HttpGet("by-patient/{patientId}")]
        [ProducesResponseType(typeof(List<RecommendationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPatient(int patientId)
            => Ok(await _sender.Send(new GetRecommendationsByPatientQuery(patientId)));

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RecommendationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _sender.Send(new GetRecommendationByIdQuery(id));
            if (result == null) return NotFound(new ErrorResponse { Message = "Recomendación no encontrada." });
            return Ok(result);
        }
    }
}
