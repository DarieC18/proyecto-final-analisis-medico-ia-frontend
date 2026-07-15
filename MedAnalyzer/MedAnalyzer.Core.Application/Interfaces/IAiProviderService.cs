namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IAiProviderService
    {
        string ModelName { get; }
        Task<string> GenerateContentAsync(string prompt, bool jsonResponse = false, CancellationToken cancellationToken = default);
    }
}
