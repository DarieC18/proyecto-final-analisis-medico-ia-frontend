using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.MedicalDocument;
using MedAnalyzer.Core.Application.Features.MedicalDocuments.Commands;
using MedAnalyzer.Core.Application.Features.MedicalDocuments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Gestión de documentos médicos adjuntos a pacientes y citas.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Nurse")]
    public class MedicalDocumentController : ControllerBase
    {
        private readonly ISender _sender;
        public MedicalDocumentController(ISender sender) => _sender = sender;

        /// <summary>Obtiene todos los documentos médicos.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<MedicalDocumentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
            => Ok(await _sender.Send(new GetAllMedicalDocumentsQuery()));

        /// <summary>Obtiene los documentos médicos de un paciente, con el nombre del usuario que los subió.</summary>
        [HttpGet("by-patient/{patientId}")]
        [ProducesResponseType(typeof(List<MedicalDocumentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPatient(int patientId)
            => Ok(await _sender.Send(new GetMedicalDocumentsByPatientQuery(patientId)));

        /// <summary>Obtiene los documentos médicos de una cita.</summary>
        [HttpGet("by-appointment/{appointmentId}")]
        [ProducesResponseType(typeof(List<MedicalDocumentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
            => Ok(await _sender.Send(new GetMedicalDocumentsByAppointmentQuery(appointmentId)));

        /// <summary>Obtiene un documento médico por su identificador.</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MedicalDocumentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var doc = await _sender.Send(new GetMedicalDocumentByIdQuery(id));
            if (doc == null) return NotFound(new ErrorResponse { Message = "Documento no encontrado." });
            return Ok(doc);
        }

        /// <summary>
        /// Sube un documento médico (PDF, JPG o PNG, máximo 10 MB).
        /// Tipos válidos: Resultados de laboratorio, Indicaciones médicas, Historial externo, Estudios en PDF, Documentos administrativos.
        /// </summary>
        [HttpPost("upload")]
        [ProducesResponseType(typeof(MedicalDocumentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Upload(
            IFormFile file,
            [FromForm] int patientId,
            [FromForm] string fileName,
            [FromForm] string fileType,
            [FromForm] int? appointmentId = null)
        {
            if (patientId == 0)
                return BadRequest(new ErrorResponse { Message = "El paciente es requerido." });

            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest(new ErrorResponse { Message = "El nombre del archivo es requerido." });

            if (!ValidFileTypes.Contains(fileType))
                return BadRequest(new ErrorResponse { Message = $"Tipo de documento no válido. Use: {string.Join(", ", ValidFileTypes)}." });

            if (file == null || file.Length == 0)
                return BadRequest(new ErrorResponse { Message = "El archivo es requerido." });

            if (file.Length > 10 * 1024 * 1024)
                return BadRequest(new ErrorResponse { Message = "El archivo no puede superar 10 MB." });

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return BadRequest(new ErrorResponse { Message = "Formato no válido. Use PDF, JPG o PNG." });

            var uploadedByUserId = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(uploadedByUserId))
                return Unauthorized(new ErrorResponse { Message = "No se pudo identificar al usuario." });

            var result = await _sender.Send(new UploadMedicalDocumentCommand(
                file.OpenReadStream(),
                file.FileName,
                patientId,
                fileName,
                fileType,
                appointmentId,
                uploadedByUserId));

            if (result == null)
                return BadRequest(new ErrorResponse { Message = "Error al guardar el documento." });

            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>Elimina un documento médico por su identificador.</summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var uploadedByUserId = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(uploadedByUserId))
                return Unauthorized(new ErrorResponse { Message = "No se pudo identificar al usuario." });

            var success = await _sender.Send(new DeleteMedicalDocumentCommand(id, uploadedByUserId));
            if (!success) return NotFound(new ErrorResponse { Message = "Documento no encontrado." });
            return NoContent();
        }

        private static readonly string[] AllowedExtensions = [".pdf", ".jpg", ".jpeg", ".png"];

        private static readonly string[] ValidFileTypes =
        [
            "Resultados de laboratorio",
            "Indicaciones médicas",
            "Historial externo",
            "Estudios en PDF",
            "Documentos administrativos"
        ];
    }
}
