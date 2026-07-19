using System.Text;
using System.Text.Json;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Setting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MedAnalyzer.Infraestructure.Shared.Services
{
    public class GeminiAiProviderService : IAiProviderService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiSettings _settings;
        private readonly ILogger<GeminiAiProviderService> _logger;

        public GeminiAiProviderService(HttpClient httpClient, IOptions<GeminiSettings> settings, ILogger<GeminiAiProviderService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public string ModelName => _settings.Model;

        public async Task<string> GenerateContentAsync(string prompt, bool jsonResponse = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
                throw new InvalidOperationException("La API key de Gemini no está configurada (AiSettings:ApiKey).");

            var requestUrl = $"{_settings.BaseUrl.TrimEnd('/')}/{_settings.Model}:generateContent?key={_settings.ApiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[] { new { text = prompt } }
                    }
                },
                generationConfig = new
                {
                    responseMimeType = jsonResponse ? "application/json" : "text/plain",
                    temperature = 0.2
                }
            };

            using var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(requestUrl, content, cancellationToken);
            var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Gemini API respondió con error {StatusCode}: {Body}", response.StatusCode, responseText);
                throw new InvalidOperationException($"Error al invocar Gemini API: {response.StatusCode}");
            }

            using var doc = JsonDocument.Parse(responseText);
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? string.Empty;
        }
    }
}
