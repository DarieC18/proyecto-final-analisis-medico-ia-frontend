using FluentAssertions;
using MedAnalyzer.Core.Application.Dto.User;
using MedAnalyzer.Core.Application.Features.Account.Commands;
using MedAnalyzer.Core.Application.Features.Account.Validators;

namespace MedAnalyzer.Tests.Validators
{
    public class CreateUserCommandValidatorTests
    {
        private readonly CreateUserCommandValidator _validator = new();

        private static RegisterDto ValidDto() => new()
        {
            Name = "María",
            LastName = "González",
            Email = "maria@clinica.com",
            UserName = "mgonzalez",
            NumberIdentification = "0987654321",
            Password = "Clinica2024!",
            ConfirmPassword = "Clinica2024!",
            Role = "Doctor"
        };

        [Fact]
        public void Validate_ConDatosValidos_DebeSerValido()
        {
            var result = _validator.Validate(new CreateUserCommand(ValidDto(), null));
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("Administrator")]
        [InlineData("Doctor")]
        [InlineData("Nurse")]
        public void Validate_RolesValidos_DebeSerValido(string role)
        {
            var dto = ValidDto();
            dto.Role = role;
            var result = _validator.Validate(new CreateUserCommand(dto, null));
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_RolInvalido_DebeFallar()
        {
            var dto = ValidDto();
            dto.Role = "Superadmin";
            var result = _validator.Validate(new CreateUserCommand(dto, null));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("Role"));
        }

        [Fact]
        public void Validate_RolNulo_DebeSerValido()
        {
            var dto = ValidDto();
            dto.Role = null;
            var result = _validator.Validate(new CreateUserCommand(dto, null));
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ContrasenasNoCoinciden_DebeFallar()
        {
            var dto = ValidDto();
            dto.ConfirmPassword = "OtraClave999!";
            var result = _validator.Validate(new CreateUserCommand(dto, null));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("ConfirmPassword"));
        }

        [Fact]
        public void Validate_PasswordMenorA8Caracteres_DebeFallar()
        {
            var dto = ValidDto();
            dto.Password = "Abc12!";
            dto.ConfirmPassword = "Abc12!";
            var result = _validator.Validate(new CreateUserCommand(dto, null));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("Password"));
        }

        [Fact]
        public void Validate_EmailInvalido_DebeFallar()
        {
            var dto = ValidDto();
            dto.Email = "no-es-email";
            var result = _validator.Validate(new CreateUserCommand(dto, null));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("Email"));
        }
    }
}
