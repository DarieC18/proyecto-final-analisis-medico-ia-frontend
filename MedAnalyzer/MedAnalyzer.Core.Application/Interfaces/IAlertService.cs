using MedAnalyzer.Core.Application.Dto.Alert;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IAlertService : IBaseServices<Alert, AlertDto>
    {
        Task<List<AlertDto>> GetByPatientIdAsync(int patientId);
        Task<List<AlertDto>> GetByAppointmentIdAsync(int appointmentId);
        Task<List<AlertDto>> GetActiveAsync();
    }
}
