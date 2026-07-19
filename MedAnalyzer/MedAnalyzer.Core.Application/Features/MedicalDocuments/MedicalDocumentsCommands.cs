using MedAnalyzer.Core.Application.Dto.MedicalDocument;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.MedicalDocuments.Commands
{
    public record UploadMedicalDocumentCommand(
        Stream FileStream,
        string OriginalFileName,
        int PatientId,
        string FileName,
        string FileType,
        int? AppointmentId,
        string UploadedByUserId
    ) : IRequest<MedicalDocumentDto?>;

    public class UploadMedicalDocumentCommandHandler : IRequestHandler<UploadMedicalDocumentCommand, MedicalDocumentDto?>
    {
        private readonly IMedicalDocumentService _service;
        private readonly IFileStorageService _fileStorage;
        private readonly IPdfTextExtractor _pdfExtractor;

        public UploadMedicalDocumentCommandHandler(
            IMedicalDocumentService service,
            IFileStorageService fileStorage,
            IPdfTextExtractor pdfExtractor)
        {
            _service = service;
            _fileStorage = fileStorage;
            _pdfExtractor = pdfExtractor;
        }

        public async Task<MedicalDocumentDto?> Handle(UploadMedicalDocumentCommand req, CancellationToken ct)
        {
            var ext = Path.GetExtension(req.OriginalFileName).ToLowerInvariant();
            string? extractedText = null;
            if (ext == ".pdf")
                extractedText = _pdfExtractor.ExtractText(req.FileStream);

            var storedName = $"{Guid.NewGuid()}{ext}";
            var relativePath = await _fileStorage.SaveAsync(req.FileStream, req.PatientId.ToString(), storedName);

            var dto = new MedicalDocumentDto
            {
                PatientId = req.PatientId,
                AppointmentId = req.AppointmentId,
                FileName = req.FileName,
                FileType = req.FileType,
                FilePath = relativePath,
                UploadedByUserId = req.UploadedByUserId,
                UploadedAt = DateTime.UtcNow,
                ExtractedText = extractedText
            };

            return await _service.UploadDocument(dto, req.UploadedByUserId);
        }
    }

    public record DeleteMedicalDocumentCommand(int Id, string CurrentUserId) : IRequest<bool>;
    public class DeleteMedicalDocumentCommandHandler : IRequestHandler<DeleteMedicalDocumentCommand, bool>
    {
        private readonly IMedicalDocumentService _service;
        public DeleteMedicalDocumentCommandHandler(IMedicalDocumentService service) => _service = service;
        public async Task<bool> Handle(DeleteMedicalDocumentCommand req, CancellationToken ct)
            => await _service.DeleteDocumentAsync(req.Id, req.CurrentUserId);
    }
}
