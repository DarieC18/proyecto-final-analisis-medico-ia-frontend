using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.MedicalDocument;
using MedAnalyzer.Core.Application.Interfaces;
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
        private readonly IMedicalDocumentService _documentService;
        private readonly IFileStorageService _fileStorage;

        public MedicalDocumentController(IMedicalDocumentService documentService, IFileStorageService fileStorage)
        {
            _documentService = documentService;
            _fileStorage = fileStorage;
        }

        /// <summary>Obtiene los documentos médicos de un paciente.</summary>
        /// <param name="patientId">Identificador del paciente.</param>
        /// <returns>Lista de documentos médicos del paciente.</returns>
        [HttpGet("by-patient/{patientId}")]
        [ProducesResponseType(typeof(List<MedicalDocumentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            var docs = await _documentService.GetByPatientId(patientId);
            return Ok(docs ?? []);
        }

        /// <summary>Obtiene los documentos médicos de una cita.</summary>
        /// <param name="appointmentId">Identificador de la cita.</param>
        /// <returns>Lista de documentos médicos de la cita.</returns>
        [HttpGet("by-appointment/{appointmentId}")]
        [ProducesResponseType(typeof(List<MedicalDocumentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
        {
            var docs = await _documentService.GetByAppointmentId(appointmentId);
            return Ok(docs ?? []);
        }

        /// <summary>Obtiene un documento médico por su identificador.</summary>
        /// <param name="id">Identificador del documento.</param>
        /// <returns>Datos del documento médico.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MedicalDocumentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var doc = await _documentService.GetDtoById(id);
            if (doc == null)
                return NotFound(new ErrorResponse { Message = "Documento no encontrado." });
            return Ok(doc);
        }

        /// <summary>
        /// Sube un documento médico (PDF, JPG o PNG, máximo 10 MB).
        /// Tipos válidos: Resultados de laboratorio, Indicaciones médicas, Historial externo, Estudios en PDF, Documentos administrativos.
        /// </summary>
        /// <param name="file">Archivo a subir.</param>
        /// <param name="patientId">Identificador del paciente.</param>
        /// <param name="fileName">Nombre descriptivo del documento.</param>
        /// <param name="fileType">Categoría del documento.</param>
        /// <param name="appointmentId">Identificador de la cita asociada (opcional).</param>
        /// <returns>Datos del documento guardado.</returns>
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

            var storedName = $"{Guid.NewGuid()}{ext}";
            var relativePath = await _fileStorage.SaveAsync(file.OpenReadStream(), patientId.ToString(), storedName);

            var uploadedByUserId = User.FindFirstValue("uid") ?? string.Empty;

            var dto = new MedicalDocumentDto
            {
                PatientId = patientId,
                AppointmentId = appointmentId,
                FileName = fileName,
                FileType = fileType,
                FilePath = relativePath,
                UploadedByUserId = uploadedByUserId,
                UploadedAt = DateTime.UtcNow,
                ExtractedText = null
            };

            var result = await _documentService.SaveDtoAsync(dto);
            if (result == null)
                return BadRequest(new ErrorResponse { Message = "Error al guardar el documento." });

            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>Elimina un documento médico y su archivo físico.</summary>
        /// <param name="id">Identificador del documento.</param>
        /// <returns>Sin contenido si la operación fue exitosa.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var doc = await _documentService.GetDtoById(id);
            if (doc == null)
                return NotFound(new ErrorResponse { Message = "Documento no encontrado." });

            _fileStorage.Delete(doc.FilePath);

            await _documentService.DeleteHardDtoAsync(id);
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
