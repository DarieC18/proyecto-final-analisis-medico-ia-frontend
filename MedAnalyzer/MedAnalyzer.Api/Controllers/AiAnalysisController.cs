using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.AiAnalisys;
using MedAnalyzer.Core.Application.Features.AiAnalyses.Commands;
using MedAnalyzer.Core.Application.Features.AiAnalyses.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Consulta de análisis generados por inteligencia artificial.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Nurse")]
    public class AiAnalysisController : ControllerBase
    {
        private readonly ISender _sender;
        public AiAnalysisController(ISender sender) => _sender = sender;

        /// <summary>Obtiene todos los análisis de IA.</summary>
        [HttpGet]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        [ProducesResponseType(typeof(List<AiAnalisysDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
            => Ok(await _sender.Send(new GetAllAiAnalysesQuery()));

        /// <summary>Genera un nuevo análisis de IA para una cita médica usando Gemini.</summary>
        [HttpPost("generate")]
        [ProducesResponseType(typeof(AiAnalisysDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Generate([FromBody] GenerateAiAnalysisRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var currentUserId = User.FindFirstValue("uid") ?? string.Empty;
            var result = await _sender.Send(new GenerateAiAnalysisCommand(request, currentUserId));
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>Genera un nuevo análisis de IA para una cita médica usando Gemini.</summary>
        /// <param name="request">Cita y tipo de análisis a generar.</param>
        /// <returns>El análisis de IA generado.</returns>
        [HttpPost("generate")]
        [ProducesResponseType(typeof(AiAnalisysDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Generate([FromBody] GenerateAiAnalysisRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirstValue("uid") ?? string.Empty;
            var result = await _aiService.GenerateAnalysisAsync(request, currentUserId);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>Obtiene los análisis de IA asociados a una cita médica.</summary>
        [HttpGet("by-appointment/{appointmentId}")]
        [ProducesResponseType(typeof(List<AiAnalisysDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
            => Ok(await _sender.Send(new GetAiAnalysesByAppointmentQuery(appointmentId)));

        /// <summary>Obtiene todos los análisis de IA de un paciente.</summary>
        [HttpGet("by-patient/{patientId}")]
        [ProducesResponseType(typeof(List<AiAnalisysDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPatient(int patientId)
            => Ok(await _sender.Send(new GetAiAnalysesByPatientQuery(patientId)));

        /// <summary>Obtiene un análisis de IA por su identificador.</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AiAnalisysDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var analysis = await _sender.Send(new GetAiAnalysisByIdQuery(id));
            if (analysis == null) return NotFound(new ErrorResponse { Message = "Análisis de IA no encontrado." });
            return Ok(analysis);
        }

        /// <summary>Marca un análisis de IA como revisado por el médico.</summary>
        /// <param name="id">Identificador del análisis.</param>
        /// <returns>Sin contenido si la operación fue exitosa.</returns>
        [Authorize(Roles = "Doctor")]
        [HttpPatch("{id}/review")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkReviewed(int id)
        {
            var success = await _sender.Send(new MarkAiAnalysisReviewedCommand(id));
            if (!success) return NotFound(new ErrorResponse { Message = "Análisis de IA no encontrado." });
            return NoContent();
        }
    }
}
