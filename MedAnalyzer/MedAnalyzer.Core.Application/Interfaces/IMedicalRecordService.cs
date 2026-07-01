using MedAnalyzer.Core.Application.Dto.MedicalRecord;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IMedicalRecordService : IBaseServices<MedicalRecord, MedicalRecordDto>
    {
        Task<List<MedicalRecordDto>> GetByAppointmentId(int appointmentId);
        Task<List<MedicalRecordDto>> GetByPatientId(int patientId);
    }
}
