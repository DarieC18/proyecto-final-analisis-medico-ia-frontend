using FluentValidation;
using MedAnalyzer.Core.Application.Features.VitalSigns.Commands;

namespace MedAnalyzer.Core.Application.Features.VitalSigns.Validators
{
    public class RegisterVitalSignCommandValidator : AbstractValidator<RegisterVitalSignCommand>
    {
        public RegisterVitalSignCommandValidator()
        {
            RuleFor(x => x.Dto.AppointmentId).GreaterThan(0).WithMessage("Se requiere una cita válida.");

            RuleFor(x => x.Dto.Temperature)
                .InclusiveBetween(30m, 45m).When(x => x.Dto.Temperature.HasValue)
                .WithMessage("La temperatura debe estar entre 30 °C y 45 °C.");

            RuleFor(x => x.Dto.HeartRate)
                .InclusiveBetween(20, 300).When(x => x.Dto.HeartRate.HasValue)
                .WithMessage("La frecuencia cardíaca debe estar entre 20 y 300 lpm.");

            RuleFor(x => x.Dto.SystolicPressure)
                .InclusiveBetween(50, 300).When(x => x.Dto.SystolicPressure.HasValue)
                .WithMessage("La presión sistólica debe estar entre 50 y 300 mmHg.");

            RuleFor(x => x.Dto.DiastolicPressure)
                .InclusiveBetween(30, 200).When(x => x.Dto.DiastolicPressure.HasValue)
                .WithMessage("La presión diastólica debe estar entre 30 y 200 mmHg.");

            RuleFor(x => x.Dto.OxygenSaturation)
                .InclusiveBetween(50m, 100m).When(x => x.Dto.OxygenSaturation.HasValue)
                .WithMessage("La saturación de oxígeno debe estar entre 50% y 100%.");

            RuleFor(x => x.Dto.Glucose)
                .InclusiveBetween(10m, 1000m).When(x => x.Dto.Glucose.HasValue)
                .WithMessage("La glucosa debe estar entre 10 y 1000 mg/dL.");
        }
    }

    public class UpdateVitalSignCommandValidator : AbstractValidator<UpdateVitalSignCommand>
    {
        public UpdateVitalSignCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("El ID de los signos vitales es inválido.");

            RuleFor(x => x.Dto.Temperature)
                .InclusiveBetween(30m, 45m).When(x => x.Dto.Temperature.HasValue)
                .WithMessage("La temperatura debe estar entre 30 °C y 45 °C.");

            RuleFor(x => x.Dto.HeartRate)
                .InclusiveBetween(20, 300).When(x => x.Dto.HeartRate.HasValue)
                .WithMessage("La frecuencia cardíaca debe estar entre 20 y 300 lpm.");

            RuleFor(x => x.Dto.SystolicPressure)
                .InclusiveBetween(50, 300).When(x => x.Dto.SystolicPressure.HasValue)
                .WithMessage("La presión sistólica debe estar entre 50 y 300 mmHg.");

            RuleFor(x => x.Dto.DiastolicPressure)
                .InclusiveBetween(30, 200).When(x => x.Dto.DiastolicPressure.HasValue)
                .WithMessage("La presión diastólica debe estar entre 30 y 200 mmHg.");

            RuleFor(x => x.Dto.OxygenSaturation)
                .InclusiveBetween(50m, 100m).When(x => x.Dto.OxygenSaturation.HasValue)
                .WithMessage("La saturación de oxígeno debe estar entre 50% y 100%.");

            RuleFor(x => x.Dto.Glucose)
                .InclusiveBetween(10m, 1000m).When(x => x.Dto.Glucose.HasValue)
                .WithMessage("La glucosa debe estar entre 10 y 1000 mg/dL.");
        }
    }
}
