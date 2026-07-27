using MedAnalyzer.Core.Application.Dto.User;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.Account.Commands
{
    public record CreateUserCommand(RegisterDto Dto, string? CurrentUserId) : IRequest<RegisterResponseDto>;
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, RegisterResponseDto>
    {
        private readonly IAccountServiceForWebApi _service;
        private readonly IAuditLogService _auditLogService;
        public CreateUserCommandHandler(IAccountServiceForWebApi service, IAuditLogService auditLogService)
        {
            _service = service;
            _auditLogService = auditLogService;
        }
        public async Task<RegisterResponseDto> Handle(CreateUserCommand req, CancellationToken ct)
        {
            var response = await _service.RegisterAsync(req.Dto, req.CurrentUserId);
            if (!response.HasError && !string.IsNullOrEmpty(req.CurrentUserId))
                await _auditLogService.LogAsync(req.CurrentUserId, "CreateUser", "AppUser", response.Id);
            return response;
        }
    }

    public record UpdateUserCommand(string Id, UpdateUserDto Dto) : IRequest<UserResponseDto>;
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponseDto>
    {
        private readonly IAccountServiceForWebApi _service;
        public UpdateUserCommandHandler(IAccountServiceForWebApi service) => _service = service;
        public async Task<UserResponseDto> Handle(UpdateUserCommand req, CancellationToken ct)
            => await _service.UpdateUserAsync(req.Id, req.Dto);
    }

    public record ToggleUserStatusCommand(string Id, string CurrentUserId) : IRequest<UserResponseDto>;
    public class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand, UserResponseDto>
    {
        private readonly IAccountServiceForWebApi _service;
        private readonly IAuditLogService _auditLogService;
        public ToggleUserStatusCommandHandler(IAccountServiceForWebApi service, IAuditLogService auditLogService)
        {
            _service = service;
            _auditLogService = auditLogService;
        }
        public async Task<UserResponseDto> Handle(ToggleUserStatusCommand req, CancellationToken ct)
        {
            var response = await _service.CambiarEstadoAsync(req.Id);
            if (!response.HasError)
                await _auditLogService.LogAsync(req.CurrentUserId, "ToggleStatus", "AppUser", req.Id);
            return response;
        }
    }

    public record DeleteUserCommand(string Id, string CurrentUserId) : IRequest<UserResponseDto>;
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, UserResponseDto>
    {
        private readonly IAccountServiceForWebApi _service;
        private readonly IAuditLogService _auditLogService;
        public DeleteUserCommandHandler(IAccountServiceForWebApi service, IAuditLogService auditLogService)
        {
            _service = service;
            _auditLogService = auditLogService;
        }
        public async Task<UserResponseDto> Handle(DeleteUserCommand req, CancellationToken ct)
        {
            var response = await _service.DeleteAsync(req.Id);
            if (!response.HasError)
                await _auditLogService.LogAsync(req.CurrentUserId, "Delete", "AppUser", req.Id);
            return response;
        }
    }
}
