using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.Patient;
using MedAnalyzer.Core.Application.Features.Patients.Commands;
using MedAnalyzer.Core.Application.Features.Patients.Queries;
using MedAnalyzer.Core.Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Gestión de pacientes. Creación y consulta de expedientes clínicos.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly ISender _sender;
        public PatientController(ISender sender) => _sender = sender;

        /// <summary>Obtiene todos los pacientes activos.</summary>
        [HttpGet]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(List<PatientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _sender.Send(new GetAllPatientsQuery());
            if (result == null || result.Count == 0) return NoContent();
            return Ok(result);
        }

        /// <summary>Busca pacientes por teléfono, tipo o UserId.</summary>
        [HttpGet("search")]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(List<PatientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var result = await _sender.Send(new SearchPatientsQuery(query));
            if (result == null || result.Count == 0) return NoContent();
            return Ok(result);
        }

        /// <summary>Obtiene un paciente por ID.</summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _sender.Send(new GetPatientByIdQuery(id));
            if (result == null) return NotFound(new ErrorResponse { Message = "Paciente no encontrado." });
            return Ok(result);
        }

        /// <summary>Obtiene el detalle completo del paciente incluyendo historial y documentos.</summary>
        [HttpGet("{id}/details")]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(PatientDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetails(int id)
        {
            var result = await _sender.Send(new GetPatientDetailQuery(id));
            if (result == null) return NotFound(new ErrorResponse { Message = "Paciente no encontrado." });
            return Ok(result);
        }

        /// <summary>El doctor crea un paciente. Se crea automáticamente su cuenta de portal y se envían credenciales por correo.</summary>
        [HttpPost("create")]
        [Authorize(Roles = "Doctor")]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePatientByDoctorDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var currentUser = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(currentUser))
                return Unauthorized(new ErrorResponse { Message = "No se pudo identificar al usuario." });

            var result = await _sender.Send(new CreatePatientCommand(dto, currentUser));
            if (result == null) return BadRequest(new ErrorResponse { Message = "Error al crear el expediente del paciente." });
            return StatusCode(201, result);
        }

        /// <summary>Actualiza los datos clínicos de un paciente.</summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] PatientDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var currentUser = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(currentUser))
                return Unauthorized(new ErrorResponse { Message = "No se pudo identificar al usuario." });

            var result = await _sender.Send(new UpdatePatientCommand(dto, id, currentUser));
            if (result == null) return BadRequest(new ErrorResponse { Message = "Error al actualizar el paciente." });
            return Ok(result);
        }

        /// <summary>Desactiva el expediente clínico de un paciente.</summary>
        [HttpPatch("{id}/deactivate")]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(int id)
        {
            var currentUserId = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized(new ErrorResponse { Message = "No se pudo identificar al usuario." });

            var result = await _sender.Send(new DeactivatePatientCommand(id, currentUserId));
            if (result == DesactivatePatient.NotFound) return NotFound(new ErrorResponse { Message = "Paciente no encontrado." });
            if (result == DesactivatePatient.HasActiveAppointments) return BadRequest(new ErrorResponse { Message = "El paciente tiene citas activas." });
            if (result == DesactivatePatient.Failed) return BadRequest(new ErrorResponse { Message = "Error al desactivar el paciente." });
            return Ok(new MessageResponse { Message = "Paciente desactivado exitosamente." });
        }

        /// <summary>Elimina permanentemente un paciente.</summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = User.FindFirstValue("uid") ?? "";
            var result = await _sender.Send(new DeletePatientCommand(id, currentUserId));
            if (!result) return BadRequest(new ErrorResponse { Message = "No se puede eliminar el paciente. Verifique que no tenga citas o documentos asociados." });
            return NoContent();
        }
    }
}
