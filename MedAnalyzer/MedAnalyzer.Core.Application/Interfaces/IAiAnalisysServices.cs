using MedAnalyzer.Core.Application.Dto.AiAnalisys;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IAiAnalisysServices : IBaseServices<AiAnalysis, AiAnalisysDto>
    {
        Task<List<AiAnalisysDto>> GetByAppointmentIdAsync(int appointmentId);
        Task<List<AiAnalisysDto>> GetByPatientIdAsync(int patientId);
    }
}
