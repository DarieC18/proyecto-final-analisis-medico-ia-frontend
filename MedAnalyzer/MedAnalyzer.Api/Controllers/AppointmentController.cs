using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.Appointment;
using MedAnalyzer.Core.Application.Features.Appointments.Commands;
using MedAnalyzer.Core.Application.Features.Appointments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Gestión de citas médicas.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Nurse")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IAccountServiceForWebApi _accountService; 

        public AppointmentController(IAppointmentService appointmentService, IAccountServiceForWebApi accountService)
        {
            _appointmentService = appointmentService;
            _accountService = accountService;
        }

        /// <summary>Obtiene las citas del médico autenticado.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<AppointmentListItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll()
        {
            var uid = CurrentUserId;
            if (string.IsNullOrEmpty(uid)) return Unauthorized(new ErrorResponse { Message = "No se pudo identificar al usuario." });
            return Ok(await _sender.Send(new GetAppointmentsByDoctorQuery(uid)));
        }

        /// <summary>Filtra las citas del médico autenticado por paciente y/o estado.</summary>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(List<AppointmentListItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetFiltered([FromQuery] int? patientId, [FromQuery] string? status)
        {
            var uid = CurrentUserId;
            if (string.IsNullOrEmpty(uid)) return Unauthorized(new ErrorResponse { Message = "No se pudo identificar al usuario." });
            return Ok(await _sender.Send(new GetFilteredAppointmentsQuery(uid, patientId, status)));
        }

        /// <summary>Obtiene todas las citas de un paciente específico.</summary>
        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(typeof(List<AppointmentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPatient(int patientId)
            => Ok(await _sender.Send(new GetAppointmentsByPatientQuery(patientId)));

        /// <summary>Obtiene una cita médica por su identificador.</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _sender.Send(new GetAppointmentByIdQuery(id));
            if (result == null) return NotFound(new ErrorResponse { Message = "Cita no encontrada." });
            return Ok(result);
        }

        /// <summary>Obtiene el detalle completo de una cita incluyendo signos vitales, síntomas y registros clínicos.</summary>
        [HttpGet("{id}/detail")]
        [ProducesResponseType(typeof(AppointmentDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _sender.Send(new GetAppointmentDetailQuery(id));
            if (result == null) return NotFound(new ErrorResponse { Message = "Cita no encontrada." });
            return Ok(result);
        }

        /// <summary>Crea una nueva cita médica.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] AppointmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var uid = CurrentUserId;
            if (string.IsNullOrEmpty(uid)) return Unauthorized(new ErrorResponse { Message = "No se pudo identificar al usuario." });
            dto.DoctorId = uid;
            var result = await _sender.Send(new CreateAppointmentCommand(dto, uid));
            if (result == null) return BadRequest(new ErrorResponse { Message = "Error al crear la cita." });
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>Actualiza los datos de una cita médica existente.</summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] AppointmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var uid = CurrentUserId;
            if (string.IsNullOrEmpty(uid)) return Unauthorized(new ErrorResponse { Message = "No se pudo identificar al usuario." });
            var result = await _sender.Send(new UpdateAppointmentCommand(dto, id, uid));
            if (result == null) return NotFound(new ErrorResponse { Message = "Cita no encontrada." });
            return Ok(result);
        }

        /// <summary>Cambia el estado de una cita médica. Estados válidos: Pending, InProgress, Completed, Cancelled.</summary>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeAppointmentStatusDto dto)
        {
            var uid = CurrentUserId;
            if (string.IsNullOrEmpty(uid)) return Unauthorized(new ErrorResponse { Message = "No se pudo identificar al usuario." });
            var result = await _sender.Send(new ChangeAppointmentStatusCommand(id, dto.Status, uid));
            if (result == null) return NotFound(new ErrorResponse { Message = "Cita no encontrada." });
            return Ok(new MessageResponse { Message = $"Estado actualizado a '{dto.Status}' correctamente." });
        }

        [HttpGet("{id}/consult")]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(AppointmentConsultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetConsultDetail(int id)
        {
            var consult = await _appointmentService.GetConsultDetail(id);
            if (consult == null)
                return NotFound(new ErrorResponse { Message = "Cita no encontrada." });

            var doctor = await _accountService.GetUserById(consult.DoctorId);
            if (doctor != null)
                consult.DoctorName = $"{doctor.Name} {doctor.LastName}";

            return Ok(consult);
        }
    }
}
