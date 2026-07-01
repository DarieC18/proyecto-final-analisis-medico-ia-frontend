using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.AiAnalisys;
using MedAnalyzer.Core.Application.Dto.Appointment;
using MedAnalyzer.Core.Application.Dto.Recommendation;
using MedAnalyzer.Core.Application.Dto.Report;
using MedAnalyzer.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Generación de reportes clínicos consolidados.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Nurse")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>Genera un reporte detallado de una cita médica incluyendo todos sus datos clínicos.</summary>
        /// <param name="id">Identificador de la cita.</param>
        /// <returns>Detalle completo de la cita para el reporte.</returns>
        [HttpGet("appointment/{id}")]
        [ProducesResponseType(typeof(AppointmentDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAppointmentReport(int id)
        {
            var detail = await _reportService.GetAppointmentReportAsync(id);
            if (detail == null)
                return NotFound(new ErrorResponse { Message = "Cita no encontrada." });
            return Ok(detail);
        }

        /// <summary>Genera un reporte completo de un paciente con su historial y alertas.</summary>
        /// <param name="id">Identificador del paciente.</param>
        /// <returns>Reporte del paciente con sus datos clínicos y alertas.</returns>
        [HttpGet("patient/{id}")]
        [ProducesResponseType(typeof(PatientReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPatientReport(int id)
        {
            var report = await _reportService.GetPatientReportAsync(id);
            if (report == null)
                return NotFound(new ErrorResponse { Message = "Paciente no encontrado." });
            return Ok(report);
        }

        /// <summary>Obtiene todos los análisis de IA de una cita para incluir en reportes.</summary>
        /// <param name="appointmentId">Identificador de la cita.</param>
        /// <returns>Lista de análisis de IA de la cita.</returns>
        [HttpGet("ai-analyses/{appointmentId}")]
        [ProducesResponseType(typeof(List<AiAnalisysDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAiAnalysesReport(int appointmentId)
        {
            var list = await _reportService.GetAiAnalysesReportAsync(appointmentId);
            return Ok(list);
        }

        /// <summary>Obtiene todas las recomendaciones de un paciente para incluir en reportes.</summary>
        /// <param name="patientId">Identificador del paciente.</param>
        /// <returns>Lista de recomendaciones del paciente.</returns>
        [HttpGet("recommendations/{patientId}")]
        [ProducesResponseType(typeof(List<RecommendationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecommendationsReport(int patientId)
        {
            var list = await _reportService.GetRecommendationsReportAsync(patientId);
            return Ok(list);
        }
    }
}
