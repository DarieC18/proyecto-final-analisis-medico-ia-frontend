using MedAnalyzer.Core.Application.Dto.MedicalRecord;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.MedicalRecords.Queries
{
    public record GetMedicalRecordsByAppointmentQuery(int AppointmentId) : IRequest<List<MedicalRecordDto>>;
    public class GetMedicalRecordsByAppointmentQueryHandler : IRequestHandler<GetMedicalRecordsByAppointmentQuery, List<MedicalRecordDto>>
    {
        private readonly IMedicalRecordService _service;
        public GetMedicalRecordsByAppointmentQueryHandler(IMedicalRecordService service) => _service = service;
        public async Task<List<MedicalRecordDto>> Handle(GetMedicalRecordsByAppointmentQuery req, CancellationToken ct)
            => await _service.GetByAppointmentId(req.AppointmentId) ?? [];
    }

    public record GetMedicalRecordsByPatientQuery(int PatientId) : IRequest<List<MedicalRecordDto>>;
    public class GetMedicalRecordsByPatientQueryHandler : IRequestHandler<GetMedicalRecordsByPatientQuery, List<MedicalRecordDto>>
    {
        private readonly IMedicalRecordService _service;
        public GetMedicalRecordsByPatientQueryHandler(IMedicalRecordService service) => _service = service;
        public async Task<List<MedicalRecordDto>> Handle(GetMedicalRecordsByPatientQuery req, CancellationToken ct)
            => await _service.GetByPatientId(req.PatientId) ?? [];
    }

    public record GetMedicalRecordByIdQuery(int Id) : IRequest<MedicalRecordDto?>;
    public class GetMedicalRecordByIdQueryHandler : IRequestHandler<GetMedicalRecordByIdQuery, MedicalRecordDto?>
    {
        private readonly IMedicalRecordService _service;
        public GetMedicalRecordByIdQueryHandler(IMedicalRecordService service) => _service = service;
        public async Task<MedicalRecordDto?> Handle(GetMedicalRecordByIdQuery req, CancellationToken ct)
            => await _service.GetDtoById(req.Id);
    }

    public record GetMedicalRecordSummariesByPatientQuery(int PatientId) : IRequest<List<MedicalRecordSummaryDto>?>;
    public class GetMedicalRecordSummariesByPatientQueryHandler : IRequestHandler<GetMedicalRecordSummariesByPatientQuery, List<MedicalRecordSummaryDto>?>
    {
        private readonly IMedicalRecordService _service;
        public GetMedicalRecordSummariesByPatientQueryHandler(IMedicalRecordService service) => _service = service;
        public async Task<List<MedicalRecordSummaryDto>?> Handle(GetMedicalRecordSummariesByPatientQuery req, CancellationToken ct)
            => await _service.GetSummariesByPatientId(req.PatientId);
    }
}
