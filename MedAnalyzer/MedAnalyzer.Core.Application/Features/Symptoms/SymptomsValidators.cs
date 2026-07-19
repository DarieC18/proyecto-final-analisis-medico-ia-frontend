using FluentValidation;
using MedAnalyzer.Core.Application.Features.Symptoms.Commands;

namespace MedAnalyzer.Core.Application.Features.Symptoms.Validators
{
    public class CreateSymptomCommandValidator : AbstractValidator<CreateSymptomCommand>
    {
        private static readonly string[] ValidSeverities = ["Leve", "Moderado", "Severo"];

        public CreateSymptomCommandValidator()
        {
            RuleFor(x => x.Dto.AppointmentId).GreaterThan(0).WithMessage("Se requiere una cita válida.");
            RuleFor(x => x.Dto.Name).NotEmpty().MaximumLength(200)
                .WithMessage("El nombre del síntoma es requerido y no debe superar 200 caracteres.");
            RuleFor(x => x.Dto.Severity).Must(s => ValidSeverities.Contains(s))
                .WithMessage($"La severidad debe ser una de: {string.Join(", ", ValidSeverities)}.");
            RuleFor(x => x.Dto.Notes).MaximumLength(500).WithMessage("Las notas no deben superar 500 caracteres.");
        }
    }

    public class UpdateSymptomCommandValidator : AbstractValidator<UpdateSymptomCommand>
    {
        private static readonly string[] ValidSeverities = ["Leve", "Moderado", "Severo"];

        public UpdateSymptomCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("El ID del síntoma es inválido.");
            RuleFor(x => x.Dto.Name).NotEmpty().MaximumLength(200)
                .WithMessage("El nombre del síntoma es requerido y no debe superar 200 caracteres.");
            RuleFor(x => x.Dto.Severity).Must(s => ValidSeverities.Contains(s))
                .WithMessage($"La severidad debe ser una de: {string.Join(", ", ValidSeverities)}.");
        }
    }
}
