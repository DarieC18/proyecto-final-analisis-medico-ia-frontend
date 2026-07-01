using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.Patient;
using MedAnalyzer.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Gestión de pacientes del sistema.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        /// <summary>Obtiene la lista de todos los pacientes activos.</summary>
        /// <returns>Lista de pacientes activos.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<PatientDto>), StatusCodes.Status200OK)]
        [Authorize(Roles = "Doctor,Nurse")]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _patientService.GetActivePatients();
            return Ok(patients);
        }

        /// <summary>Busca pacientes por nombre o número de identificación.</summary>
        /// <param name="query">Texto a buscar.</param>
        /// <returns>Lista de pacientes que coinciden con la búsqueda.</returns>
        [HttpGet("search")]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(List<PatientDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var patients = await _patientService.SearchPatients(query);
            return Ok(patients);
        }

        /// <summary>Obtiene un paciente por su identificador.</summary>
        /// <param name="id">Identificador del paciente.</param>
        /// <returns>Datos del paciente.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _patientService.GetDtoById(id);
            if (patient == null)
                return NotFound(new ErrorResponse { Message = "Paciente no encontrado." });
            return Ok(patient);
        }

        /// <summary>Obtiene el detalle completo de un paciente incluyendo su historial.</summary>
        /// <param name="id">Identificador del paciente.</param>
        /// <returns>Detalle completo del paciente.</returns>
        [HttpGet("{id}/details")]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(PatientDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetails(int id)
        {
            var detail = await _patientService.GetPatientDetail(id);
            if (detail == null)
                return NotFound(new ErrorResponse { Message = "Paciente no encontrado." });
            return Ok(detail);
        }

        /// <summary>Registra un nuevo paciente en el sistema.</summary>
        /// <param name="dto">Datos del paciente a registrar.</param>
        /// <returns>Datos del paciente creado.</returns>
        [HttpPost]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] PatientDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _patientService.SaveDtoAsync(dto);
            if (result == null)
                return BadRequest(new ErrorResponse { Message = "El número de identificación ya está registrado." });

            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>Actualiza los datos de un paciente existente.</summary>
        /// <param name="id">Identificador del paciente.</param>
        /// <param name="dto">Nuevos datos del paciente.</param>
        /// <returns>Datos del paciente actualizado.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor,Nurse")]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] PatientDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _patientService.UpdateDtoAsync(dto, id);
            if (result == null)
                return BadRequest(new ErrorResponse { Message = "Error al actualizar el paciente." });

            return Ok(result);
        }

        /// <summary>Realiza una baja lógica de un paciente.</summary>
        /// <param name="id">Identificador del paciente.</param>
        /// <returns>Sin contenido si la operación fue exitosa.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor,Nurse,Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _patientService.SoftDeletePatient(id);
            if (!result)
                return NotFound(new ErrorResponse { Message = "Paciente no encontrado." });

            return NoContent();
        }
    }
}
