using FluentValidation;
using MedAnalyzer.Core.Application.Features.Appointments.Commands;

namespace MedAnalyzer.Core.Application.Features.Appointments.Validators
{
    public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
    {
        private static readonly string[] ValidStatuses = ["Pending", "InProgress", "Completed", "Cancelled"];

        public CreateAppointmentCommandValidator()
        {
            RuleFor(x => x.Dto.PatientId).GreaterThan(0).WithMessage("Se requiere un paciente válido.");
            RuleFor(x => x.Dto.AppointmentDate).GreaterThan(DateTime.UtcNow.AddMinutes(-5))
                .WithMessage("La fecha de la cita no puede ser en el pasado.");
            RuleFor(x => x.Dto.Reason).NotEmpty().MaximumLength(500)
                .WithMessage("El motivo de la cita es requerido y no debe superar 500 caracteres.");
            RuleFor(x => x.Dto.Status).Must(s => ValidStatuses.Contains(s))
                .WithMessage($"El estado debe ser uno de: {string.Join(", ", ValidStatuses)}.");
            RuleFor(x => x.Dto.Notes).MaximumLength(1000).WithMessage("Las notas no deben superar 1000 caracteres.");
        }
    }

    public class UpdateAppointmentCommandValidator : AbstractValidator<UpdateAppointmentCommand>
    {
        public UpdateAppointmentCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("El ID de la cita es inválido.");
            RuleFor(x => x.Dto.Reason).NotEmpty().MaximumLength(500)
                .WithMessage("El motivo de la cita es requerido y no debe superar 500 caracteres.");
            RuleFor(x => x.Dto.Notes).MaximumLength(1000).WithMessage("Las notas no deben superar 1000 caracteres.");
        }
    }

    public class ChangeAppointmentStatusCommandValidator : AbstractValidator<ChangeAppointmentStatusCommand>
    {
        private static readonly string[] ValidStatuses = ["Pending", "InProgress", "Completed", "Cancelled"];

        public ChangeAppointmentStatusCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("El ID de la cita es inválido.");
            RuleFor(x => x.Status).Must(s => ValidStatuses.Contains(s))
                .WithMessage($"El estado debe ser uno de: {string.Join(", ", ValidStatuses)}.");
        }
    }
}
