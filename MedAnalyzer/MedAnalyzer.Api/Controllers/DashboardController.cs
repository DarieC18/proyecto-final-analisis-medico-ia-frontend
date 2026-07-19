using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.Dashboard;
using MedAnalyzer.Core.Application.Features.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Estadísticas y métricas del panel de control.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ISender _sender;
        public DashboardController(ISender sender) => _sender = sender;

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(AdminDashboardDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAdminStats()
            => Ok(await _sender.Send(new GetAdminDashboardQuery()));

        [HttpGet("doctor")]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(DoctorDashboardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDoctorStats()
        {
            var userId = User.FindFirstValue("uid");
            if (userId == null) return Unauthorized(new ErrorResponse { Message = "No estás autenticado." });
            var result = await _sender.Send(new GetDoctorDashboardQuery(userId));
            if (result == null) return NotFound(new ErrorResponse { Message = "Error al obtener datos del dashboard." });
            return Ok(result);
        }
    }
}
