using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.VitalSign;
using MedAnalyzer.Core.Application.Features.VitalSigns.Commands;
using MedAnalyzer.Core.Application.Features.VitalSigns.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Gestión de signos vitales registrados durante una cita médica.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Nurse")]
    public class VitalSignController : ControllerBase
    {
        private readonly ISender _sender;
        public VitalSignController(ISender sender) => _sender = sender;

        [HttpGet("by-appointment/{appointmentId}")]
        [ProducesResponseType(typeof(List<VitalSignDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
            => Ok(await _sender.Send(new GetVitalSignsByAppointmentQuery(appointmentId)));

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VitalSignDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _sender.Send(new GetVitalSignByIdQuery(id));
            if (result == null) return NotFound(new ErrorResponse { Message = "Signos vitales no encontrados." });
            return Ok(result);
        }

        /// <summary>Registra signos vitales en una cita y genera alertas automáticas para valores fuera de rango.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(VitalSignDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] VitalSignDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var uid = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(uid)) return Unauthorized(new ErrorResponse { Message = "No se pudo identificar al usuario." });
            var result = await _sender.Send(new RegisterVitalSignCommand(dto, uid));
            if (result == null) return BadRequest(new ErrorResponse { Message = "Error al registrar los signos vitales." });
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(VitalSignDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] VitalSignDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _sender.Send(new UpdateVitalSignCommand(dto, id));
            if (result == null) return NotFound(new ErrorResponse { Message = "Signos vitales no encontrados." });
            return Ok(result);
        }
    }
}
