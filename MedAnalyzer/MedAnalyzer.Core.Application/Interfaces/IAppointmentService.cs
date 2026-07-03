using MedAnalyzer.Core.Application.Dto.Appointment;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IAppointmentService : IBaseServices<Appointment, AppointmentDto>
    {
        Task<AppointmentDetailDto?> GetAppointmentDetail(int id);
        Task<AppointmentDto?> ChangeStatusAsync(int id, string status);
    }
}
