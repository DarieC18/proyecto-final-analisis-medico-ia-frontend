using MedAnalyzer.Core.Application.Dto.Patient;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IPatientService : IBaseServices<Patient,PatientDto>
    {
        Task<List<PatientDto>> GetActivePatients();
        Task<List<PatientDto>> SearchPatients(string search);
        Task<PatientDetailDto?> GetPatientDetail(int id);
        Task<bool> SoftDeletePatient(int id);
    }
}
