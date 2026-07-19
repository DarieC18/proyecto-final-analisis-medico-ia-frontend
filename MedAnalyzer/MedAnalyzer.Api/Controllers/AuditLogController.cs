using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.AuditLog;
using MedAnalyzer.Core.Application.Features.AuditLogs.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Consulta de registros de auditoría (solo administradores).</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class AuditLogController : ControllerBase
    {
        private readonly ISender _sender;
        public AuditLogController(ISender sender) => _sender = sender;

        [HttpGet]
        [ProducesResponseType(typeof(List<AuditLogDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
            => Ok(await _sender.Send(new GetAllAuditLogsQuery()));

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AuditLogDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _sender.Send(new GetAuditLogByIdQuery(id));
            if (result == null) return NotFound(new ErrorResponse { Message = "Registro de auditoría no encontrado." });
            return Ok(result);
        }

        [HttpGet("filter")]
        [ProducesResponseType(typeof(List<AuditLogDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFiltered(
            [FromQuery] string? userId,
            [FromQuery] string? action,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
            => Ok(await _sender.Send(new GetFilteredAuditLogsQuery(userId, action, from, to)));
    }
}
