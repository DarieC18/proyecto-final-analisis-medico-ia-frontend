using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.Appointment;
using MedAnalyzer.Core.Application.Interfaces;
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

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        /// <summary>Obtiene todas las citas médicas registradas.</summary>
        /// <returns>Lista de citas médicas.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<AppointmentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _appointmentService.GetAllListDto();
            return Ok(appointments);
        }

        /// <summary>Obtiene una cita médica por su identificador.</summary>
        /// <param name="id">Identificador de la cita.</param>
        /// <returns>Datos de la cita médica.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _appointmentService.GetDtoById(id);
            if (appointment == null)
                return NotFound(new ErrorResponse { Message = "Cita no encontrada." });
            return Ok(appointment);
        }

        /// <summary>Obtiene el detalle completo de una cita incluyendo signos vitales, síntomas y registros clínicos.</summary>
        /// <param name="id">Identificador de la cita.</param>
        /// <returns>Detalle completo de la cita médica.</returns>
        [HttpGet("{id}/detail")]
        [ProducesResponseType(typeof(AppointmentDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetail(int id)
        {
            var detail = await _appointmentService.GetAppointmentDetail(id);
            if (detail == null)
                return NotFound(new ErrorResponse { Message = "Cita no encontrada." });
            return Ok(detail);
        }

        /// <summary>Crea una nueva cita médica.</summary>
        /// <param name="dto">Datos de la cita a crear.</param>
        /// <returns>Datos de la cita creada.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] AppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirstValue("uid");
            if (string.IsNullOrWhiteSpace(dto.DoctorId))
                dto.DoctorId = currentUserId ?? "";

            var result = await _appointmentService.SaveDtoAsync(dto);
            if (result == null)
                return BadRequest(new ErrorResponse { Message = "Error al crear la cita." });

            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>Actualiza los datos de una cita médica existente.</summary>
        /// <param name="id">Identificador de la cita.</param>
        /// <param name="dto">Nuevos datos de la cita.</param>
        /// <returns>Datos de la cita actualizada.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] AppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _appointmentService.UpdateDtoAsync(dto, id);
            if (result == null)
                return NotFound(new ErrorResponse { Message = "Cita no encontrada." });

            return Ok(result);
        }

        /// <summary>Cambia el estado de una cita médica. Estados válidos: Pending, InProgress, Completed, Cancelled.</summary>
        /// <param name="id">Identificador de la cita.</param>
        /// <param name="dto">Nuevo estado de la cita.</param>
        /// <returns>Mensaje de confirmación y el nuevo estado.</returns>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeAppointmentStatusDto dto)
        {
            var result = await _appointmentService.ChangeStatusAsync(id, dto.Status);
            if (result == null)
                return NotFound(new ErrorResponse { Message = "Cita no encontrada." });

            return Ok(new MessageResponse { Message = $"Estado actualizado a '{dto.Status}' correctamente." });
        }
    }
}
