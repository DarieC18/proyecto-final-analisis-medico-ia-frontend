using MedAnalyzer.Core.Application.Dto.Patient;
using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Enum;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IPatientService : IBaseServices<Patient, PatientDto>
    {
        Task<List<PatientDto>> GetActivePatients();
        Task<List<PatientDto>> SearchPatients(string search);
        Task<PatientDetailDto?> GetPatientDetail(int id);
        Task<DesactivatePatient> DeactivatePatient(int id);
        Task<bool> DeletePatient(int id);
        Task<PatientDto?> UpdatePatientAsync(PatientDto dto, int id);
        Task<PatientDto?> GetByUserId(string userId);
        Task LinkUserAsync(int patientId, string userId);
    }
}
