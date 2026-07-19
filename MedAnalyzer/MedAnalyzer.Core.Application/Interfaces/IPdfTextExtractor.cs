namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IPdfTextExtractor
    {
        string? ExtractText(Stream pdfStream);
    }
}
