using FluentValidation;
using MedAnalyzer.Core.Application.Features.Account.Commands;

namespace MedAnalyzer.Core.Application.Features.Account.Validators
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private static readonly string[] ValidRoles = ["Administrator", "Doctor", "Nurse"];

        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Dto.Name).NotEmpty().MaximumLength(100)
                .WithMessage("El nombre es requerido y no debe superar 100 caracteres.");
            RuleFor(x => x.Dto.LastName).NotEmpty().MaximumLength(100)
                .WithMessage("El apellido es requerido y no debe superar 100 caracteres.");
            RuleFor(x => x.Dto.Email).NotEmpty().EmailAddress()
                .WithMessage("Se requiere un correo electrónico válido.");
            RuleFor(x => x.Dto.UserName).NotEmpty().MaximumLength(50)
                .WithMessage("El nombre de usuario es requerido y no debe superar 50 caracteres.");
            RuleFor(x => x.Dto.NumberIdentification).NotEmpty().MaximumLength(20)
                .WithMessage("El número de identificación es requerido.");
            RuleFor(x => x.Dto.Password).NotEmpty().MinimumLength(8)
                .WithMessage("La contraseña debe tener al menos 8 caracteres.");
            RuleFor(x => x.Dto.ConfirmPassword).Equal(x => x.Dto.Password)
                .WithMessage("Las contraseñas no coinciden.");
            RuleFor(x => x.Dto.Role).Must(r => string.IsNullOrEmpty(r) || ValidRoles.Contains(r))
                .WithMessage($"El rol debe ser uno de: {string.Join(", ", ValidRoles)}.");
        }
    }
}
