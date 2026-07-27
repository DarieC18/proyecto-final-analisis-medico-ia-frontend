using MedAnalyzer.Core.Application.Dto.MedicalDocument;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.MedicalDocuments.Queries
{
    public record GetAllMedicalDocumentsQuery : IRequest<List<MedicalDocumentDto>>;
    public class GetAllMedicalDocumentsQueryHandler : IRequestHandler<GetAllMedicalDocumentsQuery, List<MedicalDocumentDto>>
    {
        private readonly IMedicalDocumentService _service;
        public GetAllMedicalDocumentsQueryHandler(IMedicalDocumentService service) => _service = service;
        public async Task<List<MedicalDocumentDto>> Handle(GetAllMedicalDocumentsQuery _, CancellationToken ct)
            => await _service.GetAllListDto();
    }

    public record GetMedicalDocumentsByPatientQuery(int PatientId) : IRequest<List<MedicalDocumentDto>>;
    public class GetMedicalDocumentsByPatientQueryHandler : IRequestHandler<GetMedicalDocumentsByPatientQuery, List<MedicalDocumentDto>>
    {
        private readonly IMedicalDocumentService _service;
        private readonly IBaseAccountService _accountService;
        public GetMedicalDocumentsByPatientQueryHandler(IMedicalDocumentService service, IBaseAccountService accountService)
        {
            _service = service;
            _accountService = accountService;
        }
        public async Task<List<MedicalDocumentDto>> Handle(GetMedicalDocumentsByPatientQuery req, CancellationToken ct)
        {
            var docs = await _service.GetByPatientId(req.PatientId) ?? [];
            var userIds = docs.Select(d => d.UploadedByUserId).Distinct();
            foreach (var userId in userIds)
            {
                var user = await _accountService.GetUserById(userId);
                if (user == null) continue;
                foreach (var doc in docs.Where(d => d.UploadedByUserId == userId))
                    doc.UploadedByUserName = $"{user.Name} {user.LastName}";
            }
            return docs;
        }
    }

    public record GetMedicalDocumentsByAppointmentQuery(int AppointmentId) : IRequest<List<MedicalDocumentDto>>;
    public class GetMedicalDocumentsByAppointmentQueryHandler : IRequestHandler<GetMedicalDocumentsByAppointmentQuery, List<MedicalDocumentDto>>
    {
        private readonly IMedicalDocumentService _service;
        public GetMedicalDocumentsByAppointmentQueryHandler(IMedicalDocumentService service) => _service = service;
        public async Task<List<MedicalDocumentDto>> Handle(GetMedicalDocumentsByAppointmentQuery req, CancellationToken ct)
            => await _service.GetByAppointmentId(req.AppointmentId) ?? [];
    }

    public record GetMedicalDocumentByIdQuery(int Id) : IRequest<MedicalDocumentDto?>;
    public class GetMedicalDocumentByIdQueryHandler : IRequestHandler<GetMedicalDocumentByIdQuery, MedicalDocumentDto?>
    {
        private readonly IMedicalDocumentService _service;
        public GetMedicalDocumentByIdQueryHandler(IMedicalDocumentService service) => _service = service;
        public async Task<MedicalDocumentDto?> Handle(GetMedicalDocumentByIdQuery req, CancellationToken ct)
            => await _service.GetDtoById(req.Id);
    }
}
