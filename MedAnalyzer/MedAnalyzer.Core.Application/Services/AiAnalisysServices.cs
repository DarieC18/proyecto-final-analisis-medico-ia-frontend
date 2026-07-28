using System.Text;
using System.Text.Json;
using AutoMapper;
using MedAnalyzer.Core.Application.Base;
using MedAnalyzer.Core.Application.Dto.AiAnalisys;
using MedAnalyzer.Core.Application.Dto.Recommendation;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Enum;
using MedAnalyzer.Core.Domain.Exceptions;
using MedAnalyzer.Core.Domain.Interfaces;

namespace MedAnalyzer.Core.Application.Services
{
    public class AiAnalisysServices : BaseServices<AiAnalysis, AiAnalisysDto>, IAiAnalisysServices
    {
        private readonly IBaseRepository<AiAnalysis> _repository;
        private readonly IMapper _mapper;
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        private readonly IRecommendationService _recommendationService;
        private readonly IAiProviderService _aiProvider;
        private readonly IBaseAccountService _accountService;
        private readonly IAuditLogService _auditLogService;
        private readonly IAlertService _alertService;

        public AiAnalisysServices(
            IMapper mapper,
            IBaseRepository<AiAnalysis> repository,
            IAppointmentService appointmentService,
            IPatientService patientService,
            IRecommendationService recommendationService,
            IAiProviderService aiProvider,
            IBaseAccountService accountService,
            IAuditLogService auditLogService,
            IAlertService alertService) : base(mapper, repository)
        {
            _repository = repository;
            _mapper = mapper;
            _appointmentService = appointmentService;
            _patientService = patientService;
            _recommendationService = recommendationService;
            _aiProvider = aiProvider;
            _accountService = accountService;
            _auditLogService = auditLogService;
            _alertService = alertService;
        }

        public async Task<List<AiAnalisysDto>> GetByAppointmentIdAsync(int appointmentId)
        {
            var all = await _repository.GetAllListAsync();
            return _mapper.Map<List<AiAnalisysDto>>(all.Where(a => a.AppointmentId == appointmentId).ToList());
        }

        public async Task<List<AiAnalisysDto>> GetByPatientIdAsync(int patientId)
        {
            var all = await _repository.GetAllListAsync();
            return _mapper.Map<List<AiAnalisysDto>>(all.Where(a => a.PatientId == patientId).ToList());
        }

        public async Task<AiAnalisysDto> GenerateAnalysisAsync(GenerateAiAnalysisRequestDto request, string requestedByUserId)
        {
            var appointment = await _appointmentService.GetAppointmentDetail(request.AppointmentId)
                ?? throw new DomainValidationException("La cita indicada no existe.");

            var patient = await _patientService.GetDtoById(appointment.PatientId)
                ?? throw new DomainValidationException("El paciente asociado a la cita no existe.");

            var patientUser = await _accountService.GetUserById(patient.UserId);
            var patientFullName = patientUser != null ? $"{patientUser.Name} {patientUser.LastName}" : $"Paciente #{patient.Id}";

            var prompt = BuildPrompt(request.AnalysisType, patient, patientFullName, appointment);

            var aiAnalysis = new AiAnalysis
            {
                Id = 0,
                PatientId = appointment.PatientId,
                AppointmentId = appointment.Id,
                DocumentId = appointment.Documents.FirstOrDefault()?.Id,
                RequestedByUserId = requestedByUserId,
                AnalysisType = request.AnalysisType,
                PromptUsed = prompt,
                ModelUsed = _aiProvider.ModelName,
                AiResponse = string.Empty,
                Status = AiAnalysisStatus.Pending.ToString(),
                IsReviewed = false
            };

            GeminiAnalysisResult? result = null;
            try
            {
                var rawResponse = await _aiProvider.GenerateContentAsync(prompt, jsonResponse: true);
                aiAnalysis.AiResponse = rawResponse;
                result = JsonSerializer.Deserialize<GeminiAnalysisResult>(rawResponse, JsonOptions);
                aiAnalysis.Status = AiAnalysisStatus.Approved.ToString();
            }
            catch (Exception)
            {
                aiAnalysis.Status = AiAnalysisStatus.Rejected.ToString();
            }

            var saved = await _repository.SaveEntityAsync(aiAnalysis);

            if (result?.Recommendations is { Count: > 0 })
            {
                foreach (var rec in result.Recommendations)
                {
                    await _recommendationService.SaveDtoAsync(new RecommendationDto
                    {
                        Id = 0,
                        AiAnalysisId = saved.Id,
                        AppointmentId = appointment.Id,
                        Title = rec.Title ?? "Recomendación",
                        Description = rec.Description ?? string.Empty,
                        RiskLevel = result.RiskLevel ?? "Desconocido"
                    });
                }
            }

            if (result?.RiskLevel is "Medio" or "Alto")
            {
                await _alertService.SaveDtoAsync(new Dto.Alert.AlertDto
                {
                    Id = 0,
                    PatientId = saved.PatientId,
                    AppointmentId = saved.AppointmentId,
                    Title = $"Riesgo {result.RiskLevel} detectado por análisis IA",
                    Description = result.Summary ?? "El análisis de IA identificó un nivel de riesgo que requiere revisión médica.",
                    Severity = result.RiskLevel == "Alto" ? "Crítico" : "Moderado",
                    IsResolved = false
                });
            }

            await _auditLogService.LogAsync(requestedByUserId, "GenerateAiAnalysis", "AiAnalysis", saved.Id.ToString());

            return _mapper.Map<AiAnalisysDto>(saved);
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static string BuildPrompt(string analysisType, Dto.Patient.PatientDto patient, string patientFullName, Dto.Appointment.AppointmentDetailDto appointment)
        {
            var age = DateTime.UtcNow.Year - patient.BirthDate.Year;
            var sb = new StringBuilder();

            sb.AppendLine("Eres un asistente clínico de apoyo diagnóstico. NO reemplazas el criterio médico.");
            sb.AppendLine($"Tipo de análisis solicitado: {analysisType}.");
            sb.AppendLine();
            sb.AppendLine($"Paciente: {patientFullName}, {age} años, género {patient.Gender}.");
            sb.AppendLine($"Motivo de la cita: {appointment.Reason}.");
            if (!string.IsNullOrWhiteSpace(appointment.Notes))
                sb.AppendLine($"Notas de la cita: {appointment.Notes}");

            if (appointment.Symptoms.Count > 0)
            {
                sb.AppendLine("Síntomas reportados:");
                foreach (var s in appointment.Symptoms)
                    sb.AppendLine($"- {s.Name} (severidad: {s.Severity}, desde: {s.StartedAt})");
            }

            if (appointment.VitalSigns.Count > 0)
            {
                sb.AppendLine("Signos vitales:");
                foreach (var v in appointment.VitalSigns)
                    sb.AppendLine($"- Temp: {v.Temperature}°C, FC: {v.HeartRate}bpm, PA: {v.SystolicPressure}/{v.DiastolicPressure}, SpO2: {v.OxygenSaturation}%, Glucosa: {v.Glucose?.ToString() ?? "N/A"}");
            }

            if (appointment.MedicalRecords.Count > 0)
            {
                sb.AppendLine("Historial clínico relevante:");
                foreach (var r in appointment.MedicalRecords)
                    sb.AppendLine($"- Diagnóstico inicial: {r.DiagnosisInitial}. Notas: {r.Notes}. Antecedentes: {r.Antecedentes}");
            }

            if (appointment.Documents.Count > 0)
            {
                sb.AppendLine("Contenido extraído de documentos médicos adjuntos:");
                foreach (var d in appointment.Documents)
                {
                    if (!string.IsNullOrWhiteSpace(d.ExtractedText))
                        sb.AppendLine($"- {d.FileName}: {d.ExtractedText}");
                }
            }

            sb.AppendLine();
            sb.AppendLine("Responde ÚNICAMENTE en formato JSON con esta estructura exacta:");
            sb.AppendLine("{ \"summary\": string, \"riskLevel\": \"Bajo\"|\"Medio\"|\"Alto\", \"recommendations\": [ { \"title\": string, \"description\": string } ] }");

            return sb.ToString();
        }

        private class GeminiAnalysisResult
        {
            public string? Summary { get; set; }
            public string? RiskLevel { get; set; }
            public List<GeminiRecommendation>? Recommendations { get; set; }
        }

        private class GeminiRecommendation
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
        }
    }
}
