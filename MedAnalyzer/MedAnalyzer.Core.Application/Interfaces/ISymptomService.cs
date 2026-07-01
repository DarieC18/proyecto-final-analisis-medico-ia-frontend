using MedAnalyzer.Core.Application.Dto.Symptom;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface ISymptomService : IBaseServices<Symptom, SymptomDto>
    {
        Task<List<SymptomDto>> GetByAppointmentId(int appointmentId);
    }
}
