using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.User;
using MedAnalyzer.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedAnalyzer.Api.Controllers
{
    /// <summary>Autenticación y gestión de sesión de usuarios.</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountServiceForWebApi _accountService;
        private readonly IAuditLogService _auditLogService;

        public AuthController(IAccountServiceForWebApi accountService, IAuditLogService auditLogService)
        {
            _accountService = accountService;
            _auditLogService = auditLogService;
        }

        /// <summary>Registra un nuevo usuario en el sistema.</summary>
        /// <param name="dto">Datos de registro del usuario.</param>
        /// <returns>Resultado del registro con información del usuario creado.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirstValue("uid");
            var response = await _accountService.RegisterAsync(dto, currentUserId);

            if (response.HasError)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>Autentica a un usuario y devuelve un token JWT.</summary>
        /// <param name="dto">Credenciales de acceso.</param>
        /// <returns>Token de acceso y datos del usuario autenticado.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponseForApiDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginResponseForApiDto), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _accountService.AuthenticateAsync(dto);

            if (response.HasError)
                return Unauthorized(response);

            await _auditLogService.LogAsync(response.Id, "Login", "AppUser", response.Id);

            return Ok(response);
        }

        /// <summary>Confirma el correo electrónico de un usuario mediante token.</summary>
        /// <param name="userId">Identificador del usuario.</param>
        /// <param name="token">Token de confirmación enviado por correo.</param>
        /// <returns>Mensaje con el resultado de la confirmación.</returns>
        [HttpGet("confirm-account")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmAccount([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return BadRequest(new ErrorResponse { Message = "El userId y el token son requeridos." });

            var response = await _accountService.ConfirmAccountAsync(userId, token);

            if (response.HasError)
                return BadRequest(new ErrorResponse
                {
                    Message = response.Errors.FirstOrDefault() ?? "Error al confirmar la cuenta.",
                    Errors = response.Errors
                });

            return Ok(new MessageResponse { Message = "Cuenta confirmada exitosamente." });
        }

        /// <summary>Solicita el envío de un correo para restablecer la contraseña.</summary>
        /// <param name="dto">Correo electrónico del usuario.</param>
        /// <returns>Mensaje de confirmación del envío.</returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _accountService.ForgotPasswordAsync(dto);

            if (response.HasError)
                return BadRequest(response);

            return Ok(new MessageResponse { Message = "Si el usuario existe, recibirá un correo para restablecer su contraseña." });
        }

        /// <summary>Restablece la contraseña de un usuario mediante token.</summary>
        /// <param name="dto">Token de restablecimiento y nueva contraseña.</param>
        /// <returns>Mensaje de confirmación del restablecimiento.</returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _accountService.ResetPasswordAsync(dto);

            if (response.HasError)
                return BadRequest(response);

            return Ok(new MessageResponse { Message = "Contraseña restablecida exitosamente." });
        }

        /// <summary>Obtiene los datos del usuario actualmente autenticado.</summary>
        /// <returns>Datos del usuario en sesión.</returns>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue("uid");
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new ErrorResponse { Message = "No estás autenticado." });

            var user = await _accountService.GetUserById(userId);
            if (user == null)
                return NotFound(new ErrorResponse { Message = "Usuario no encontrado." });

            return Ok(user);
        }
    }
}
