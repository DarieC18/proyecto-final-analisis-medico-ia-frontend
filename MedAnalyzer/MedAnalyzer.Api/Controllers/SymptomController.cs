using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.Symptom;
using MedAnalyzer.Core.Application.Features.Symptoms.Commands;
using MedAnalyzer.Core.Application.Features.Symptoms.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Gestión de síntomas registrados durante una cita médica.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Nurse")]
    public class SymptomController : ControllerBase
    {
        private readonly ISender _sender;
        public SymptomController(ISender sender) => _sender = sender;

        [HttpGet("by-appointment/{appointmentId}")]
        [ProducesResponseType(typeof(List<SymptomDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
            => Ok(await _sender.Send(new GetSymptomsByAppointmentQuery(appointmentId)));

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SymptomDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _sender.Send(new GetSymptomByIdQuery(id));
            if (result == null) return NotFound(new ErrorResponse { Message = "Síntoma no encontrado." });
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(SymptomDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] SymptomDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var uid = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(uid)) return Unauthorized(new ErrorResponse { Message = "No se pudo identificar al usuario." });
            var result = await _sender.Send(new CreateSymptomCommand(dto, uid));
            if (result == null) return BadRequest(new ErrorResponse { Message = "Error al registrar el síntoma." });
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SymptomDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] SymptomDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _sender.Send(new UpdateSymptomCommand(dto, id));
            if (result == null) return NotFound(new ErrorResponse { Message = "Síntoma no encontrado." });
            return Ok(result);
        }
    }
}
