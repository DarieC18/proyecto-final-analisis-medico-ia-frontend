using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.AiChat;
using MedAnalyzer.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Chat de consulta médica general asistido por IA.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Nurse")]
    public class AiChatController : ControllerBase
    {
        private readonly IAiChatService _aiChatService;

        public AiChatController(IAiChatService aiChatService)
        {
            _aiChatService = aiChatService;
        }

        /// <summary>Realiza una pregunta de consulta médica general a la IA. Preguntas fuera del ámbito médico son rechazadas.</summary>
        /// <param name="request">Pregunta a realizar.</param>
        /// <returns>Respuesta generada por la IA.</returns>
        [HttpPost("ask")]
        [ProducesResponseType(typeof(AiChatResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Ask([FromBody] AiChatRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _aiChatService.AskAsync(request);
            return Ok(result);
        }
    }
}
