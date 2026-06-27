using MedAnalyzer.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedAnalyzer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var logs = await _auditLogService.GetAllListDto();

            if (logs == null || logs.Count == 0)
                return NoContent();

            return Ok(logs);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var log = await _auditLogService.GetDtoById(id);

            if (log == null)
                return NoContent();

            return Ok(log);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFiltered(
            [FromQuery] string? userId,
            [FromQuery] string? action,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var logs = await _auditLogService.GetFilteredAsync(userId, action, from, to);

            if (logs == null || logs.Count == 0)
                return NoContent();

            return Ok(logs);
        }
    }
}