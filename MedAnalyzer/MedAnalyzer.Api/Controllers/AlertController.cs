using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.Alert;
using MedAnalyzer.Core.Application.Features.Alerts.Commands;
using MedAnalyzer.Core.Application.Features.Alerts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Gestión de alertas clínicas generadas por el sistema.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Nurse")]
    public class AlertController : ControllerBase
    {
        private readonly ISender _sender;
        public AlertController(ISender sender) => _sender = sender;

        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        /// <summary>Obtiene todas las alertas registradas.</summary>
        /// <returns>Lista de todas las alertas.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<AlertDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var alerts = await _alertService.GetAllListDto();
            return Ok(alerts);
        }

        /// <summary>Obtiene las alertas asociadas a un paciente.</summary>
        /// <param name="patientId">Identificador del paciente.</param>
        /// <returns>Lista de alertas del paciente.</returns>
        [HttpGet("by-patient/{patientId}")]
        [ProducesResponseType(typeof(List<AlertDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPatient(int patientId)
            => Ok(await _sender.Send(new GetAlertsByPatientQuery(patientId)));

        [HttpGet("by-appointment/{appointmentId}")]
        [ProducesResponseType(typeof(List<AlertDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
            => Ok(await _sender.Send(new GetAlertsByAppointmentQuery(appointmentId)));

        [HttpGet("active")]
        [ProducesResponseType(typeof(List<AlertDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActive()
            => Ok(await _sender.Send(new GetActiveAlertsQuery()));

        [HttpPatch("{id}/resolve")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Resolve(int id)
        {
            var uid = User.FindFirstValue("uid") ?? "";
            var result = await _sender.Send(new ResolveAlertCommand(id, uid));
            if (!result) return NotFound(new ErrorResponse { Message = "Alerta no encontrada." });
            return NoContent();
        }
    }
}
