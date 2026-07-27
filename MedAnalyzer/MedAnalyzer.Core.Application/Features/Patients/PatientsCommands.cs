using MedAnalyzer.Core.Application.Dto.Patient;
using MedAnalyzer.Core.Application.Interfaces;
using MedAnalyzer.Core.Domain.Enum;
using MedAnalyzer.Core.Domain.Exceptions;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.Patients.Commands
{
    public record CreatePatientCommand(CreatePatientByDoctorDto Dto, string CurrentUserId) : IRequest<PatientDto?>;
    public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, PatientDto?>
    {
        private readonly IPatientService _service;
        private readonly IAccountServiceForWebApi _accountService;
        private readonly IEmailService _emailService;
        public CreatePatientCommandHandler(IPatientService service, IAccountServiceForWebApi accountService, IEmailService emailService)
        {
            _service = service;
            _accountService = accountService;
            _emailService = emailService;
        }
        public async Task<PatientDto?> Handle(CreatePatientCommand req, CancellationToken ct)
        {
            string userId, resetToken;
            try
            {
                (userId, resetToken) = await _accountService.RegisterPatientAccountAsync(
                    req.Dto.Email, req.Dto.FirstName, req.Dto.LastName, req.Dto.NumberIdentification);
            }
            catch (InvalidOperationException ex)
            {
                throw new DomainValidationException(ex.Message);
            }

            var patientDto = new PatientDto
            {
                Id = 0,
                UserId = userId,
                PhoneNumber = req.Dto.PhoneNumber,
                Gender = req.Dto.Gender,
                BirthDate = req.Dto.BirthDate,
                IdentificationType = req.Dto.IdentificationType,
                PatientType = req.Dto.PatientType,
                IsActive = true
            };

            var result = await _service.CreatePatient(patientDto, req.CurrentUserId);
            if (result == null)
            {
                await _accountService.DeleteAsync(userId);
                return null;
            }

            await _emailService.SendPatientActivationEmailAsync(req.Dto.Email, req.Dto.FirstName, userId, resetToken);
            return result;
        }
    }

    public record UpdatePatientCommand(PatientDto Dto, int Id, string CurrentUserId) : IRequest<PatientDto?>;
    public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, PatientDto?>
    {
        private readonly IPatientService _service;
        public UpdatePatientCommandHandler(IPatientService service) => _service = service;
        public async Task<PatientDto?> Handle(UpdatePatientCommand req, CancellationToken ct)
            => await _service.UpdatePatientAsync(req.Dto, req.Id, req.CurrentUserId);
    }

    public record DeactivatePatientCommand(int Id, string CurrentUserId) : IRequest<DesactivatePatient>;
    public class DeactivatePatientCommandHandler : IRequestHandler<DeactivatePatientCommand, DesactivatePatient>
    {
        private readonly IPatientService _service;
        public DeactivatePatientCommandHandler(IPatientService service) => _service = service;
        public async Task<DesactivatePatient> Handle(DeactivatePatientCommand req, CancellationToken ct)
            => await _service.DeactivatePatient(req.Id, req.CurrentUserId);
    }

    public record DeletePatientCommand(int Id, string CurrentUserId) : IRequest<bool>;
    public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, bool>
    {
        private readonly IPatientService _service;
        private readonly IAuditLogService _auditLogService;
        public DeletePatientCommandHandler(IPatientService service, IAuditLogService auditLogService)
        {
            _service = service;
            _auditLogService = auditLogService;
        }
        public async Task<bool> Handle(DeletePatientCommand req, CancellationToken ct)
        {
            var result = await _service.DeletePatient(req.Id);
            if (result)
                await _auditLogService.LogAsync(req.CurrentUserId, "DeletePatient", "Patient", req.Id.ToString());
            return result;
        }
    }
}
