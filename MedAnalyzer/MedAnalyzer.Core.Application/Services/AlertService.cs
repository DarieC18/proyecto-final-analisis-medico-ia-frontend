using AutoMapper;
using MedAnalyzer.Core.Application.Base;
using MedAnalyzer.Core.Application.Dto.Alert;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Interfaces;

namespace MedAnalyzer.Core.Application.Services
{
    public class AlertService : BaseServices<Alert, AlertDto>, IAlertService
    {
        private readonly IBaseRepository<Alert> _repository;
        private readonly IMapper _mapper;

        public AlertService(IBaseRepository<Alert> repository, IMapper mapper) : base(mapper, repository)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<AlertDto>> GetByPatientIdAsync(int patientId)
        {
            var all = await _repository.GetAllListAsync();
            return _mapper.Map<List<AlertDto>>(all.Where(a => a.PatientId == patientId).ToList());
        }

        public async Task<List<AlertDto>> GetByAppointmentIdAsync(int appointmentId)
        {
            var all = await _repository.GetAllListAsync();
            return _mapper.Map<List<AlertDto>>(all.Where(a => a.AppointmentId == appointmentId).ToList());
        }

        public async Task<List<AlertDto>> GetActiveAsync()
        {
            var all = await _repository.GetAllListAsync();
            return _mapper.Map<List<AlertDto>>(all.Where(a => !a.IsResolved).ToList());
        }
    }
}
