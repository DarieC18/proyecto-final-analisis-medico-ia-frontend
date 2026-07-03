using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.User;
using MedAnalyzer.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Gestión de cuentas de usuario (solo administradores).</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServiceForWebApi _accountService;
        private readonly IAuditLogService _auditLogService;

        public AccountController(IAccountServiceForWebApi accountService, IAuditLogService auditLogService)
        {
            _accountService = accountService;
            _auditLogService = auditLogService;
        }

        /// <summary>Obtiene la lista de todos los usuarios del sistema.</summary>
        /// <returns>Lista de usuarios registrados.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var users = await _accountService.GetAllUser(null);
            return Ok(users);
        }

        /// <summary>Crea un nuevo usuario en el sistema.</summary>
        /// <param name="dto">Datos del nuevo usuario.</param>
        /// <returns>Datos del usuario creado.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirstValue("uid");
            var response = await _accountService.RegisterAsync(dto, currentUserId);

            if (response.HasError)
                return BadRequest(response);

            return StatusCode(StatusCodes.Status201Created, response);
        }

        /// <summary>Actualiza los datos de un usuario existente.</summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <param name="dto">Nuevos datos del usuario.</param>
        /// <returns>Datos del usuario actualizado.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _accountService.UpdateUserAsync(id, dto);

            if (response.HasError)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>Activa o desactiva la cuenta de un usuario.</summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <returns>Estado actualizado del usuario.</returns>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var currentUserId = User.FindFirstValue("uid");
            if (currentUserId == id)
                return BadRequest(new ErrorResponse { Message = "No puedes inactivar tu propia cuenta." });

            var response = await _accountService.CambiarEstadoAsync(id);

            if (response.HasError)
                return BadRequest(response);

            await _auditLogService.LogAsync(currentUserId!, "ToggleStatus", "AppUser", id);

            return Ok(response);
        }

        /// <summary>Elimina un usuario del sistema.</summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string id)
        {
            var currentUserId = User.FindFirstValue("uid");
            var response = await _accountService.DeleteAsync(id);

            if (response.HasError)
                return BadRequest(response);

            await _auditLogService.LogAsync(currentUserId!, "Delete", "AppUser", id);

            return Ok(response);
        }
    }
}
