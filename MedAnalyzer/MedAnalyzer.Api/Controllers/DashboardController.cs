using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.Dashboard;
using MedAnalyzer.Core.Application.Interfaces;
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
        private readonly IAdminDashboardService _adminDashboardService;
        private readonly IDashboardService<DoctorDashboardDto> _doctorDashboardService;

        public DashboardController(
            IAdminDashboardService adminDashboardService,
            IDashboardService<DoctorDashboardDto> doctorDashboardService)
        {
            _adminDashboardService = adminDashboardService;
            _doctorDashboardService = doctorDashboardService;
        }

        /// <summary>Obtiene las estadísticas generales del sistema para el panel del administrador.</summary>
        /// <returns>Métricas globales: usuarios, pacientes, citas y alertas.</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(AdminDashboardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAdminStats()
        {
            var stats = await _adminDashboardService.GetDashboardStatsAsync();
            return Ok(stats);
        }

        /// <summary>Obtiene las estadísticas personalizadas para el panel del médico o enfermero autenticado.</summary>
        /// <returns>Métricas del usuario: citas, pacientes y alertas propias.</returns>
        [HttpGet("doctor")]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(DoctorDashboardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDoctorStats()
        {
            var userId = User.FindFirstValue("uid");

            if (userId == null)
                return Unauthorized(new ErrorResponse { Message = "No estás autenticado." });

            var result = await _doctorDashboardService.GetDashboard(userId);

            if (result == null)
                return NotFound(new ErrorResponse { Message = "Error al obtener datos del dashboard." });

            return Ok(result);
        }
    }
}
