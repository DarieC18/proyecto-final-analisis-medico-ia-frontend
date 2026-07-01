namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(Stream content, string subfolder, string fileName);
        void Delete(string relativePath);
    }
}
