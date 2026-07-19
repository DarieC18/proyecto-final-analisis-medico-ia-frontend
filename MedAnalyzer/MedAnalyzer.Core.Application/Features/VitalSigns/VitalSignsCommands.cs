using MedAnalyzer.Core.Application.Dto.VitalSign;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.VitalSigns.Commands
{
    public record RegisterVitalSignCommand(VitalSignDto Dto, string CurrentUserId) : IRequest<VitalSignDto?>;
    public class RegisterVitalSignCommandHandler : IRequestHandler<RegisterVitalSignCommand, VitalSignDto?>
    {
        private readonly IVitalSignService _service;
        public RegisterVitalSignCommandHandler(IVitalSignService service) => _service = service;
        public async Task<VitalSignDto?> Handle(RegisterVitalSignCommand req, CancellationToken ct)
        {
            if (req.Dto.MeasuredAt == default) req.Dto.MeasuredAt = DateTime.UtcNow;
            return await _service.RegisterWithAlerts(req.Dto, req.CurrentUserId);
        }
    }

    public record UpdateVitalSignCommand(VitalSignDto Dto, int Id) : IRequest<VitalSignDto?>;
    public class UpdateVitalSignCommandHandler : IRequestHandler<UpdateVitalSignCommand, VitalSignDto?>
    {
        private readonly IVitalSignService _service;
        public UpdateVitalSignCommandHandler(IVitalSignService service) => _service = service;
        public async Task<VitalSignDto?> Handle(UpdateVitalSignCommand req, CancellationToken ct)
            => await _service.UpdateDtoAsync(req.Dto, req.Id);
    }
}
