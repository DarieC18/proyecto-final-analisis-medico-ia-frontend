using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.AuditLog;
using MedAnalyzer.Core.Application.Interfaces;
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
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        /// <summary>Obtiene todos los registros de auditoría.</summary>
        /// <returns>Lista de registros de auditoría.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<AuditLogDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var logs = await _auditLogService.GetAllListDto();
            return Ok(logs);
        }

        /// <summary>Obtiene un registro de auditoría por su identificador.</summary>
        /// <param name="id">Identificador del registro.</param>
        /// <returns>Registro de auditoría encontrado.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AuditLogDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var log = await _auditLogService.GetDtoById(id);

            if (log == null)
                return NotFound(new ErrorResponse { Message = "Registro de auditoría no encontrado." });

            return Ok(log);
        }

        /// <summary>Filtra registros de auditoría por usuario, acción o rango de fechas.</summary>
        /// <param name="userId">Identificador del usuario (opcional).</param>
        /// <param name="action">Nombre de la acción registrada (opcional).</param>
        /// <param name="from">Fecha de inicio del filtro (opcional).</param>
        /// <param name="to">Fecha de fin del filtro (opcional).</param>
        /// <returns>Lista filtrada de registros de auditoría.</returns>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(List<AuditLogDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFiltered(
            [FromQuery] string? userId,
            [FromQuery] string? action,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var logs = await _auditLogService.GetFilteredAsync(userId, action, from, to);
            return Ok(logs);
        }
    }
}
