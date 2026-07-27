using MedAnalyzer.Core.Application.Dto.Appointment;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IAppointmentService : IBaseServices<Appointment, AppointmentDto>
    {
        Task<AppointmentDetailDto?> GetAppointmentDetail(int id);
        Task<AppointmentDto?> ChangeStatusAsync(int id, string status, string currentUserId);
        Task<List<AppointmentDto>> GetByPatientId(int patientId);
        Task<AppointmentConsultDto?> GetConsultDetail(int appointmentId);
        Task<AppointmentDto?> CreateAppointment(AppointmentDto dto, string currentUserId);
        Task<AppointmentDto?> UpdateAppointment(AppointmentDto dto, int id, string currentUserId);
        Task<bool> DeleteAppointmentAsync(int id);
        Task<List<AppointmentListItemDto>> GetAllByDoctorAsync(string doctorId);
        Task<List<AppointmentListItemDto>> GetFilteredAsync(string doctorId, int? patientId, string? status);
    }
}
