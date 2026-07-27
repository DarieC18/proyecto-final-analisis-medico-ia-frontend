using FluentValidation;
using MedAnalyzer.Core.Application.Features.MedicalRecords.Commands;

namespace MedAnalyzer.Core.Application.Features.MedicalRecords.Validators
{
    public class CreateMedicalRecordCommandValidator : AbstractValidator<CreateMedicalRecordCommand>
    {
        public CreateMedicalRecordCommandValidator()
        {
            RuleFor(x => x.Dto.PatientId).GreaterThan(0).WithMessage("Se requiere un paciente válido.");
            RuleFor(x => x.Dto.AppointmentId).GreaterThan(0).WithMessage("Se requiere una cita válida.");
            RuleFor(x => x.Dto.DiagnosisInitial).NotEmpty().MaximumLength(500)
                .WithMessage("El diagnóstico inicial es requerido y no debe superar 500 caracteres.");
            RuleFor(x => x.Dto.Notes).NotEmpty().MaximumLength(2000)
                .WithMessage("Las notas clínicas son requeridas y no deben superar 2000 caracteres.");
            RuleFor(x => x.Dto.Antecedentes).MaximumLength(2000)
                .WithMessage("Los antecedentes no deben superar 2000 caracteres.");
            RuleFor(x => x.Dto.ObservacionesConsulta).MaximumLength(2000)
                .WithMessage("Las observaciones no deben superar 2000 caracteres.");
        }
    }

    public class UpdateMedicalRecordCommandValidator : AbstractValidator<UpdateMedicalRecordCommand>
    {
        public UpdateMedicalRecordCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("El ID del registro clínico es inválido.");
            RuleFor(x => x.Dto.DiagnosisInitial).NotEmpty().MaximumLength(500)
                .WithMessage("El diagnóstico inicial es requerido y no debe superar 500 caracteres.");
            RuleFor(x => x.Dto.Notes).NotEmpty().MaximumLength(2000)
                .WithMessage("Las notas clínicas son requeridas y no deben superar 2000 caracteres.");
        }
    }
}
