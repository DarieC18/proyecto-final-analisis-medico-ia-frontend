using MedAnalyzer.Api.Models;
using MedAnalyzer.Core.Application.Dto.User;
using MedAnalyzer.Core.Application.Features.Account.Commands;
using MedAnalyzer.Core.Application.Features.Account.Queries;
using MediatR;
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
        private readonly ISender _sender;
        public AccountController(ISender sender) => _sender = sender;

        [HttpGet]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
            => Ok(await _sender.Send(new GetAllUsersQuery()));

        [HttpPost]
        [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var currentUserId = User.FindFirstValue("uid");
            var response = await _sender.Send(new CreateUserCommand(dto, currentUserId));
            if (response.HasError) return BadRequest(response);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var response = await _sender.Send(new UpdateUserCommand(id, dto));
            if (response.HasError) return BadRequest(response);
            return Ok(response);
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var currentUserId = User.FindFirstValue("uid");
            if (currentUserId == id)
                return BadRequest(new ErrorResponse { Message = "No puedes inactivar tu propia cuenta." });
            var response = await _sender.Send(new ToggleUserStatusCommand(id, currentUserId!));
            if (response.HasError) return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string id)
        {
            var currentUserId = User.FindFirstValue("uid");
            var response = await _sender.Send(new DeleteUserCommand(id, currentUserId!));
            if (response.HasError) return BadRequest(response);
            return Ok(response);
        }
    }
}
