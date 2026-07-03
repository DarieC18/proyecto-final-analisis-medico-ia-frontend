using AutoMapper;
using MedAnalyzer.Core.Application.Base;
using MedAnalyzer.Core.Application.Dto.MedicalRecord;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Interfaces;

namespace MedAnalyzer.Core.Application.Services
{
    public class MedicalRecordService : BaseServices<MedicalRecord, MedicalRecordDto>, IMedicalRecordService
    {
        private readonly IBaseRepository<MedicalRecord> _repository;
        private readonly IMapper _mapper;

        public MedicalRecordService(IMapper mapper, IBaseRepository<MedicalRecord> repository)
            : base(mapper, repository)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<MedicalRecordDto>> GetByAppointmentId(int appointmentId)
        {
            var all = await _repository.GetAllListAsync();
            return _mapper.Map<List<MedicalRecordDto>>(
                all.Where(r => r.AppointmentId == appointmentId).ToList());
        }

        public async Task<List<MedicalRecordDto>> GetByPatientId(int patientId)
        {
            var all = await _repository.GetAllListAsync();
            return _mapper.Map<List<MedicalRecordDto>>(
                all.Where(r => r.PatientId == patientId).ToList());
        }
    }
}
