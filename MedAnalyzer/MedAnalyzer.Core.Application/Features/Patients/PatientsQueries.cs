using MedAnalyzer.Core.Application.Dto.Patient;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.Patients.Queries
{
    public record GetAllPatientsQuery : IRequest<List<PatientDto>>;
    public class GetAllPatientsQueryHandler : IRequestHandler<GetAllPatientsQuery, List<PatientDto>>
    {
        private readonly IPatientService _service;
        public GetAllPatientsQueryHandler(IPatientService service) => _service = service;
        public async Task<List<PatientDto>> Handle(GetAllPatientsQuery _, CancellationToken ct)
            => await _service.GetActivePatients();
    }

    public record SearchPatientsQuery(string Query) : IRequest<List<PatientDto>>;
    public class SearchPatientsQueryHandler : IRequestHandler<SearchPatientsQuery, List<PatientDto>>
    {
        private readonly IPatientService _service;
        public SearchPatientsQueryHandler(IPatientService service) => _service = service;
        public async Task<List<PatientDto>> Handle(SearchPatientsQuery req, CancellationToken ct)
            => await _service.SearchPatients(req.Query);
    }

    public record GetPatientByIdQuery(int Id) : IRequest<PatientDto?>;
    public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, PatientDto?>
    {
        private readonly IPatientService _service;
        public GetPatientByIdQueryHandler(IPatientService service) => _service = service;
        public async Task<PatientDto?> Handle(GetPatientByIdQuery req, CancellationToken ct)
            => await _service.GetDtoById(req.Id);
    }

    public record GetPatientDetailQuery(int Id) : IRequest<PatientDetailDto?>;
    public class GetPatientDetailQueryHandler : IRequestHandler<GetPatientDetailQuery, PatientDetailDto?>
    {
        private readonly IPatientService _service;
        private readonly IBaseAccountService _accountService;
        public GetPatientDetailQueryHandler(IPatientService service, IBaseAccountService accountService)
        {
            _service = service;
            _accountService = accountService;
        }
        public async Task<PatientDetailDto?> Handle(GetPatientDetailQuery req, CancellationToken ct)
        {
            var detail = await _service.GetPatientDetail(req.Id);
            if (detail == null) return null;
            if (!string.IsNullOrWhiteSpace(detail.UserId))
            {
                var user = await _accountService.GetUserById(detail.UserId);
                if (user != null) detail.FullName = $"{user.Name} {user.LastName}";
            }
            return detail;
        }
    }
}
