using MedAnalyzer.Core.Application.Dto.VitalSign;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IVitalSignService : IBaseServices<VitalSign, VitalSignDto>
    {
        Task<List<VitalSignDto>> GetByAppointmentId(int appointmentId);
        Task<VitalSignDto?> RegisterWithAlerts(VitalSignDto dto);
    }
}
