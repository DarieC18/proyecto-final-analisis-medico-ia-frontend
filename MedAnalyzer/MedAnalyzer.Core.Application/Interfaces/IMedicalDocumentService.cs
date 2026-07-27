using MedAnalyzer.Core.Application.Dto.MedicalDocument;
using MedAnalyzer.Core.Domain.Entities;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IMedicalDocumentService : IBaseServices<MedicalDocument, MedicalDocumentDto>
    {
        Task<List<MedicalDocumentDto>> GetByPatientId(int patientId);
        Task<List<MedicalDocumentDto>> GetByAppointmentId(int appointmentId);
        Task<MedicalDocumentDto?> UploadDocument(MedicalDocumentDto dto, string currentUserId);
        Task<bool> DeleteDocumentAsync(int id, string currentUserId);
    }
}
