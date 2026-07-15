namespace MedAnalyzer.Core.Domain.Setting
{
    public class GeminiSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public required string Model { get; set; }
        public required string BaseUrl { get; set; }
    }
}
