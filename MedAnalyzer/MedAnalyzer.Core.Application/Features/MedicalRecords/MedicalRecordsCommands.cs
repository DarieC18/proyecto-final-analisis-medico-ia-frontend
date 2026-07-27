using MedAnalyzer.Core.Application.Dto.MedicalRecord;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.MedicalRecords.Commands
{
    public record CreateMedicalRecordCommand(MedicalRecordDto Dto, string CurrentUserId) : IRequest<MedicalRecordDto?>;
    public class CreateMedicalRecordCommandHandler : IRequestHandler<CreateMedicalRecordCommand, MedicalRecordDto?>
    {
        private readonly IMedicalRecordService _service;
        private readonly IAuditLogService _auditLogService;
        public CreateMedicalRecordCommandHandler(IMedicalRecordService service, IAuditLogService auditLogService)
        {
            _service = service;
            _auditLogService = auditLogService;
        }
        public async Task<MedicalRecordDto?> Handle(CreateMedicalRecordCommand req, CancellationToken ct)
        {
            var dto = req.Dto;
            if (string.IsNullOrWhiteSpace(dto.CreatedByUserId))
                dto.CreatedByUserId = req.CurrentUserId;
            var result = await _service.SaveDtoAsync(dto);
            if (result != null)
                await _auditLogService.LogAsync(req.CurrentUserId, "CreateMedicalRecord", "MedicalRecord", result.Id.ToString());
            return result;
        }
    }

    public record UpdateMedicalRecordCommand(MedicalRecordDto Dto, int Id, string CurrentUserId) : IRequest<MedicalRecordDto?>;
    public class UpdateMedicalRecordCommandHandler : IRequestHandler<UpdateMedicalRecordCommand, MedicalRecordDto?>
    {
        private readonly IMedicalRecordService _service;
        private readonly IAuditLogService _auditLogService;
        public UpdateMedicalRecordCommandHandler(IMedicalRecordService service, IAuditLogService auditLogService)
        {
            _service = service;
            _auditLogService = auditLogService;
        }
        public async Task<MedicalRecordDto?> Handle(UpdateMedicalRecordCommand req, CancellationToken ct)
        {
            var result = await _service.UpdateDtoAsync(req.Dto, req.Id);
            if (result != null)
                await _auditLogService.LogAsync(req.CurrentUserId, "UpdateMedicalRecord", "MedicalRecord", result.Id.ToString());
            return result;
        }
    }
}
