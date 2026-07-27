using MedAnalyzer.Core.Application.Interfaces;
using System.Text;
using UglyToad.PdfPig;

namespace MedAnalyzer.Infraestructure.Shared.Services
{
    public class PdfTextExtractorService : IPdfTextExtractor
    {
        public string? ExtractText(Stream pdfStream)
        {
            try
            {
                using var document = PdfDocument.Open(pdfStream);
                var sb = new StringBuilder();
                foreach (var page in document.GetPages())
                    sb.AppendLine(page.Text);
                var text = sb.ToString().Trim();
                return string.IsNullOrWhiteSpace(text) ? null : text;
            }
            catch
            {
                return null;
            }
        }
    }
}
