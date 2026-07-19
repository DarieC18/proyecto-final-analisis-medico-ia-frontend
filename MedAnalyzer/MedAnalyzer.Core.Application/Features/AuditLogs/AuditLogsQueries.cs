using MedAnalyzer.Core.Application.Dto.AuditLog;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.AuditLogs.Queries
{
    public record GetAllAuditLogsQuery : IRequest<List<AuditLogDto>>;
    public class GetAllAuditLogsQueryHandler : IRequestHandler<GetAllAuditLogsQuery, List<AuditLogDto>>
    {
        private readonly IAuditLogService _service;
        public GetAllAuditLogsQueryHandler(IAuditLogService service) => _service = service;
        public async Task<List<AuditLogDto>> Handle(GetAllAuditLogsQuery _, CancellationToken ct)
            => await _service.GetAllListDto();
    }

    public record GetAuditLogByIdQuery(int Id) : IRequest<AuditLogDto?>;
    public class GetAuditLogByIdQueryHandler : IRequestHandler<GetAuditLogByIdQuery, AuditLogDto?>
    {
        private readonly IAuditLogService _service;
        public GetAuditLogByIdQueryHandler(IAuditLogService service) => _service = service;
        public async Task<AuditLogDto?> Handle(GetAuditLogByIdQuery req, CancellationToken ct)
            => await _service.GetDtoById(req.Id);
    }

    public record GetFilteredAuditLogsQuery(string? UserId, string? Action, DateTime? From, DateTime? To) : IRequest<List<AuditLogDto>>;
    public class GetFilteredAuditLogsQueryHandler : IRequestHandler<GetFilteredAuditLogsQuery, List<AuditLogDto>>
    {
        private readonly IAuditLogService _service;
        public GetFilteredAuditLogsQueryHandler(IAuditLogService service) => _service = service;
        public async Task<List<AuditLogDto>> Handle(GetFilteredAuditLogsQuery req, CancellationToken ct)
            => await _service.GetFilteredAsync(req.UserId, req.Action, req.From, req.To);
    }
}
