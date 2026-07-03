
namespace MedAnalyzer.Core.Application.Dto.MedicalDocument
{
    public class MedicalDocumentDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int? AppointmentId { get; set; }
        public required string UploadedByUserId { get; set; }
        public required string FileName { get; set; }
        public required string FileType { get; set; }
        public required string FilePath { get; set; }
        public string? ExtractedText { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
