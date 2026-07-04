using AutoMapper;
using MedAnalyzer.Core.Application.Base;
using MedAnalyzer.Core.Application.Dto.AiAnalisys;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Interfaces;

namespace MedAnalyzer.Core.Application.Services
{
    public class AiAnalisysServices : BaseServices<AiAnalysis, AiAnalisysDto>, IAiAnalisysServices
    {
        private readonly IBaseRepository<AiAnalysis> _repository;
        private readonly IMapper _mapper;

        public AiAnalisysServices(IMapper mapper, IBaseRepository<AiAnalysis> repository) : base(mapper, repository)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<AiAnalisysDto>> GetByAppointmentIdAsync(int appointmentId)
        {
            var all = await _repository.GetAllListAsync();
            return _mapper.Map<List<AiAnalisysDto>>(all.Where(a => a.AppointmentId == appointmentId).ToList());
        }

        public async Task<List<AiAnalisysDto>> GetByPatientIdAsync(int patientId)
        {
            var all = await _repository.GetAllListAsync();
            return _mapper.Map<List<AiAnalisysDto>>(all.Where(a => a.PatientId == patientId).ToList());
        }
    }
}
