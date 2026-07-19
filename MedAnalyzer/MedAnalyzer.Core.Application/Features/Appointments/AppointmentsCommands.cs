using MedAnalyzer.Core.Application.Dto.Appointment;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.Appointments.Commands
{
    public record CreateAppointmentCommand(AppointmentDto Dto, string CurrentUserId) : IRequest<AppointmentDto?>;
    public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, AppointmentDto?>
    {
        private readonly IAppointmentService _service;
        public CreateAppointmentCommandHandler(IAppointmentService service) => _service = service;
        public async Task<AppointmentDto?> Handle(CreateAppointmentCommand req, CancellationToken ct)
            => await _service.CreateAppointment(req.Dto, req.CurrentUserId);
    }

    public record UpdateAppointmentCommand(AppointmentDto Dto, int Id, string CurrentUserId) : IRequest<AppointmentDto?>;
    public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, AppointmentDto?>
    {
        private readonly IAppointmentService _service;
        public UpdateAppointmentCommandHandler(IAppointmentService service) => _service = service;
        public async Task<AppointmentDto?> Handle(UpdateAppointmentCommand req, CancellationToken ct)
            => await _service.UpdateAppointment(req.Dto, req.Id, req.CurrentUserId);
    }

    public record ChangeAppointmentStatusCommand(int Id, string Status, string CurrentUserId) : IRequest<AppointmentDto?>;
    public class ChangeAppointmentStatusCommandHandler : IRequestHandler<ChangeAppointmentStatusCommand, AppointmentDto?>
    {
        private readonly IAppointmentService _service;
        public ChangeAppointmentStatusCommandHandler(IAppointmentService service) => _service = service;
        public async Task<AppointmentDto?> Handle(ChangeAppointmentStatusCommand req, CancellationToken ct)
            => await _service.ChangeStatusAsync(req.Id, req.Status, req.CurrentUserId);
    }

    public record DeleteAppointmentCommand(int Id, string CurrentUserId) : IRequest<bool>;
    public class DeleteAppointmentCommandHandler : IRequestHandler<DeleteAppointmentCommand, bool>
    {
        private readonly IAppointmentService _service;
        private readonly IAuditLogService _auditLogService;
        public DeleteAppointmentCommandHandler(IAppointmentService service, IAuditLogService auditLogService)
        {
            _service = service;
            _auditLogService = auditLogService;
        }
        public async Task<bool> Handle(DeleteAppointmentCommand req, CancellationToken ct)
        {
            var result = await _service.DeleteAppointmentAsync(req.Id);
            if (result)
                await _auditLogService.LogAsync(req.CurrentUserId, "DeleteAppointment", "Appointment", req.Id.ToString());
            return result;
        }
    }
}
