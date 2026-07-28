using MedAnalyzer.Core.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace MedAnalyzer.Infraestructure.Shared.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _webRootPath;

        public FileStorageService(IWebHostEnvironment env)
        {
            _webRootPath = env.WebRootPath;
        }

        public async Task<string> SaveAsync(Stream content, string subfolder, string fileName)
        {
            var folder = Path.Combine(_webRootPath, "uploads", "medical-documents", subfolder);
            Directory.CreateDirectory(folder);

            var fullPath = Path.Combine(folder, fileName);
            await using var fileStream = new FileStream(fullPath, FileMode.Create);
            await content.CopyToAsync(fileStream);

            return $"/uploads/medical-documents/{subfolder}/{fileName}";
        }

        public void Delete(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;
            var fullPath = Path.Combine(_webRootPath, relativePath.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
