using AutoMapper;
using MedAnalyzer.Core.Application.Base;
using MedAnalyzer.Core.Application.Dto.AuditLog;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Entities;
using MedAnalyzer.Core.Domain.Interfaces;

namespace MedAnalyzer.Core.Application.Services
{

    // ============================================================
    // PENDIENTE: Integración de auditoría en otros módulos
    // ============================================================
    // Los siguientes 5 puntos requieren llamar a LogAsync desde
    // los controllers/servicios de otros módulos cuando estén listos.
    // En cada caso, inyectar IAuditLogService y llamarlo así:
    //
    //   await _auditLogService.LogAsync(
    //       userId,       <- User.FindFirst("uid")?.Value
    //       userName,     <- $"{user.FirstName} {user.LastName}"
    //       userRole,     <- User.FindFirst("roles")?.Value
    //       action,       <- ver valores estándar abajo
    //       entityName,   <- nombre de la entidad afectada
    //       entityId      <- id de la entidad afectada como string
    //   );
    //
    // [1] module-general-features (compañera):
    //     AuthenticateAsync exitoso
    //     → LogAsync(userId, userName, userRole, "Login", "AppUser", user.Id)
    //
    // [2] moduloMedicoUsers (Jean):
    //     Crear paciente exitoso
    //     → LogAsync(userId, userName, userRole, "Create", "Patient", patient.Id.ToString())
    //
    // [3] moduloMedicoUsers (Jean):
    //     Registrar cita exitosa
    //     → LogAsync(userId, userName, userRole, "Create", "Appointment", appointment.Id.ToString())
    //
    // [4] moduloMedicoUsers (Jean):
    //     Generar análisis de IA exitoso
    //     → LogAsync(userId, userName, userRole, "GenerateAI", "AiAnalysis", analysis.Id.ToString())
    //
    // [5] moduloMedicoUsers (Jean):
    //     Subir documento médico exitoso
    //     → LogAsync(userId, userName, userRole, "Upload", "MedicalDocument", document.Id.ToString())
    // ============================================================

    public class AuditLogService : BaseServices<AuditLog, AuditLogDto>, IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IMapper _mapper;
        private readonly IBaseAccountService _accountService;

        public AuditLogService(IMapper mapper, IAuditLogRepository logRepository, IBaseAccountService accountService)
            : base(mapper, logRepository)
        {
            _auditLogRepository = logRepository;
            _mapper = mapper;
            _accountService = accountService;
        }

        public async Task<List<AuditLogDto>> GetFilteredAsync(string? userId, string? action, DateTime? from, DateTime? to)
        {
            var entities = await _auditLogRepository.GetFilteredAsync(userId, action, from, to);
            return _mapper.Map<List<AuditLogDto>>(entities);
        }

        public async Task LogAsync(string userId, string action, string entityName, string entityId)
        {
            var user = await _accountService.GetUserById(userId);

            var entity = new AuditLog
            {
                Id = 0,
                UserId = userId,
                UserName = user != null ? $"{user.Name} {user.LastName}" : "Usuario desconocido",
                UserRole = user?.Role ?? "Sin rol",
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                CreatedAt = DateTime.UtcNow
            };

            await _auditLogRepository.SaveEntityAsync(entity);
        }
    }
}