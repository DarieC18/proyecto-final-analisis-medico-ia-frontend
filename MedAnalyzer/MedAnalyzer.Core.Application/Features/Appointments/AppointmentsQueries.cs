using MedAnalyzer.Core.Application.Dto.Appointment;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.Appointments.Queries
{
    public record GetAppointmentsByDoctorQuery(string DoctorId) : IRequest<List<AppointmentListItemDto>>;
    public class GetAppointmentsByDoctorQueryHandler : IRequestHandler<GetAppointmentsByDoctorQuery, List<AppointmentListItemDto>>
    {
        private readonly IAppointmentService _service;
        public GetAppointmentsByDoctorQueryHandler(IAppointmentService service) => _service = service;
        public async Task<List<AppointmentListItemDto>> Handle(GetAppointmentsByDoctorQuery req, CancellationToken ct)
            => await _service.GetAllByDoctorAsync(req.DoctorId);
    }

    public record GetFilteredAppointmentsQuery(string DoctorId, int? PatientId, string? Status) : IRequest<List<AppointmentListItemDto>>;
    public class GetFilteredAppointmentsQueryHandler : IRequestHandler<GetFilteredAppointmentsQuery, List<AppointmentListItemDto>>
    {
        private readonly IAppointmentService _service;
        public GetFilteredAppointmentsQueryHandler(IAppointmentService service) => _service = service;
        public async Task<List<AppointmentListItemDto>> Handle(GetFilteredAppointmentsQuery req, CancellationToken ct)
            => await _service.GetFilteredAsync(req.DoctorId, req.PatientId, req.Status);
    }

    public record GetAppointmentByIdQuery(int Id) : IRequest<AppointmentDto?>;
    public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, AppointmentDto?>
    {
        private readonly IAppointmentService _service;
        public GetAppointmentByIdQueryHandler(IAppointmentService service) => _service = service;
        public async Task<AppointmentDto?> Handle(GetAppointmentByIdQuery req, CancellationToken ct)
            => await _service.GetDtoById(req.Id);
    }

    public record GetAppointmentDetailQuery(int Id) : IRequest<AppointmentDetailDto?>;
    public class GetAppointmentDetailQueryHandler : IRequestHandler<GetAppointmentDetailQuery, AppointmentDetailDto?>
    {
        private readonly IAppointmentService _service;
        public GetAppointmentDetailQueryHandler(IAppointmentService service) => _service = service;
        public async Task<AppointmentDetailDto?> Handle(GetAppointmentDetailQuery req, CancellationToken ct)
            => await _service.GetAppointmentDetail(req.Id);
    }

    public record GetAppointmentsByPatientQuery(int PatientId) : IRequest<List<AppointmentDto>>;
    public class GetAppointmentsByPatientQueryHandler : IRequestHandler<GetAppointmentsByPatientQuery, List<AppointmentDto>>
    {
        private readonly IAppointmentService _service;
        public GetAppointmentsByPatientQueryHandler(IAppointmentService service) => _service = service;
        public async Task<List<AppointmentDto>> Handle(GetAppointmentsByPatientQuery req, CancellationToken ct)
            => await _service.GetByPatientId(req.PatientId);
    }

    public record GetAppointmentConsultQuery(int AppointmentId) : IRequest<AppointmentConsultDto?>;
    public class GetAppointmentConsultQueryHandler : IRequestHandler<GetAppointmentConsultQuery, AppointmentConsultDto?>
    {
        private readonly IAppointmentService _service;
        private readonly IBaseAccountService _accountService;
        public GetAppointmentConsultQueryHandler(IAppointmentService service, IBaseAccountService accountService)
        {
            _service = service;
            _accountService = accountService;
        }
        public async Task<AppointmentConsultDto?> Handle(GetAppointmentConsultQuery req, CancellationToken ct)
        {
            var consult = await _service.GetConsultDetail(req.AppointmentId);
            if (consult == null) return null;
            var doctor = await _accountService.GetUserById(consult.DoctorId);
            if (doctor != null) consult.DoctorName = $"{doctor.Name} {doctor.LastName}";
            return consult;
        }
    }
}
