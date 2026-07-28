using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Setting;
using MedAnalyzer.Infraestructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedAnalyzer.Infraestructure.Shared
{
    public static class ServiceRegistration
    {
        public static void AddSharedLayer(this IServiceCollection services, IConfiguration confi) 
        {
            #region Configurations
            services.Configure<MailSettings>(confi.GetSection("MailSettings"));
            services.Configure<GeminiSettings>(confi.GetSection("AiSettings"));
            services.Configure<AppSettings>(confi.GetSection("AppSettings"));

            #endregion

            #region Services
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddHttpClient<IAiProviderService, GeminiAiProviderService>();
            services.AddTransient<IPdfTextExtractor, PdfTextExtractorService>();
            #endregion
        }
    }
}
