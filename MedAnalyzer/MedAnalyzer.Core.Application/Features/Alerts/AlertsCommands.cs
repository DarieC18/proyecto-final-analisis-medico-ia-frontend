using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.Alerts.Commands
{
    public record ResolveAlertCommand(int Id, string CurrentUserId) : IRequest<bool>;
    public class ResolveAlertCommandHandler : IRequestHandler<ResolveAlertCommand, bool>
    {
        private readonly IAlertService _service;
        private readonly IAuditLogService _auditLogService;
        public ResolveAlertCommandHandler(IAlertService service, IAuditLogService auditLogService)
        {
            _service = service;
            _auditLogService = auditLogService;
        }
        public async Task<bool> Handle(ResolveAlertCommand req, CancellationToken ct)
        {
            var dto = await _service.GetDtoById(req.Id);
            if (dto == null) return false;
            dto.IsResolved = true;
            await _service.UpdateDtoAsync(dto, req.Id);
            await _auditLogService.LogAsync(req.CurrentUserId, "ResolveAlert", "Alert", req.Id.ToString());
            return true;
        }
    }
}
