using MedAnalyzer.Core.Application.Dto.User;
using MedAnalyzer.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedAnalyzer.Api.Controllers
{
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

        [HttpGet("confirm-account")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmAccount([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return BadRequest(new { Message = "El userId y el token son requeridos." });

            var result = await _accountService.ConfirmAccountAsync(userId, token);

            if (result.Contains("error", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { Message = result });

            return Ok(new { Message = result });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _accountService.ForgotPasswordAsync(dto);

            if (response.HasError)
                return BadRequest(response);

            return Ok(new { Message = "Si el usuario existe, recibirá un correo para restablecer su contraseña." });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _accountService.ResetPasswordAsync(dto);

            if (response.HasError)
                return BadRequest(response);

            return Ok(new { Message = "Contraseña restablecida exitosamente." });
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue("uid");
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var user = await _accountService.GetUserById(userId);
            if (user == null)
                return NotFound(new { Message = "Usuario no encontrado." });

            return Ok(user);
        }
    }
}