using MedAnalyzer.Core.Application.Dto.User;
using MedAnalyzer.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedAnalyzer.Api.Controllers
{
    [Route("api/[controller]")]
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

        [HttpGet]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var users = await _accountService.GetAllUser(null);
            return Ok(users);
        }

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

        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var currentUserId = User.FindFirstValue("uid");
            if (currentUserId == id)
                return BadRequest(new { Message = "No puedes inactivar tu propia cuenta." });

            var response = await _accountService.CambiarEstadoAsync(id);

            if (response.HasError)
                return BadRequest(response);

            await _auditLogService.LogAsync(currentUserId!, "ToggleStatus", "AppUser", id);

            return Ok(response);
        }

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
