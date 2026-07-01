using AutoMapper;
using MedAnalyzer.Core.Application.Base;
using MedAnalyzer.Core.Application.Dto.Symptom;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Exceptions;
using MedAnalyzer.Core.Domain.Interfaces;

namespace MedAnalyzer.Core.Application.Services
{
    public class SymptomService : BaseServices<Symptom, SymptomDto>, ISymptomService
    {
        private readonly IBaseRepository<Symptom> _repository;
        private readonly IMapper _mapper;

        public SymptomService(IMapper mapper, IBaseRepository<Symptom> repository)
            : base(mapper, repository)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<SymptomDto>> GetByAppointmentId(int appointmentId)
        {
            var all = await _repository.GetAllListAsync();
            return _mapper.Map<List<SymptomDto>>(
                all.Where(s => s.AppointmentId == appointmentId).ToList());
        }

        public override async Task<SymptomDto?> SaveDtoAsync(SymptomDto dto)
        {
            Validate(dto);
            return await base.SaveDtoAsync(dto);
        }

        public override async Task<SymptomDto?> UpdateDtoAsync(SymptomDto dto, int id)
        {
            Validate(dto);
            return await base.UpdateDtoAsync(dto, id);
        }

        private static readonly string[] ValidSeverities = ["Leve", "Moderado", "Severo"];

        private static void Validate(SymptomDto dto)
        {
            if (dto.AppointmentId == 0)
                throw new DomainValidationException("La cita asociada es requerida.");
            if (dto.StartedAt > DateOnly.FromDateTime(DateTime.Today))
                throw new DomainValidationException("La fecha de inicio del síntoma no puede ser una fecha futura.");
            if (!ValidSeverities.Contains(dto.Severity))
                throw new DomainValidationException("Severidad no válida. Use: Leve, Moderado o Severo.");
        }
    }
}
