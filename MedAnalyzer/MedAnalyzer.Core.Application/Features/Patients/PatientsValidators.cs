using FluentValidation;
using MedAnalyzer.Core.Application.Features.Patients.Commands;

namespace MedAnalyzer.Core.Application.Features.Patients.Validators
{
    public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
    {
        private static readonly string[] ValidGenders = ["Masculino", "Femenino", "Otro"];
        private static readonly string[] ValidIdTypes = ["Cédula", "Pasaporte", "RUC", "Otro"];
        private static readonly string[] ValidPatientTypes = ["Nuevo", "Recurrente", "Referido", "Urgencias"];

        public CreatePatientCommandValidator()
        {
            RuleFor(x => x.Dto.FirstName).NotEmpty().MaximumLength(100)
                .WithMessage("El nombre es requerido y no debe superar 100 caracteres.");
            RuleFor(x => x.Dto.LastName).NotEmpty().MaximumLength(100)
                .WithMessage("El apellido es requerido y no debe superar 100 caracteres.");
            RuleFor(x => x.Dto.Email).NotEmpty().EmailAddress()
                .WithMessage("Se requiere un correo electrónico válido.");
            RuleFor(x => x.Dto.NumberIdentification).NotEmpty().MaximumLength(20)
                .WithMessage("El número de identificación es requerido.");
            RuleFor(x => x.Dto.PhoneNumber).MaximumLength(20)
                .WithMessage("El teléfono no debe superar 20 caracteres.");
            RuleFor(x => x.Dto.Gender).Must(g => string.IsNullOrEmpty(g) || ValidGenders.Contains(g))
                .WithMessage($"El género debe ser uno de: {string.Join(", ", ValidGenders)}.");
            RuleFor(x => x.Dto.IdentificationType).Must(t => string.IsNullOrEmpty(t) || ValidIdTypes.Contains(t))
                .WithMessage($"El tipo de identificación debe ser uno de: {string.Join(", ", ValidIdTypes)}.");
            RuleFor(x => x.Dto.PatientType).Must(t => string.IsNullOrEmpty(t) || ValidPatientTypes.Contains(t))
                .WithMessage($"El tipo de paciente debe ser uno de: {string.Join(", ", ValidPatientTypes)}.");
            RuleFor(x => x.CurrentUserId).NotEmpty()
                .WithMessage("Se requiere el identificador del médico.");
        }
    }

    public class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
    {
        public UpdatePatientCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("El ID del paciente es inválido.");
            RuleFor(x => x.Dto.PhoneNumber).MaximumLength(20).WithMessage("El teléfono no debe superar 20 caracteres.");
        }
    }
}
