using FluentAssertions;
using MedAnalyzer.Core.Application.Dto.Patient;
using MedAnalyzer.Core.Application.Features.Patients.Commands;
using MedAnalyzer.Core.Application.Features.Patients.Validators;

namespace MedAnalyzer.Tests.Validators
{
    public class CreatePatientCommandValidatorTests
    {
        private readonly CreatePatientCommandValidator _validator = new();

        private static CreatePatientCommand BuildCommand(
            string firstName = "Juan",
            string lastName = "Pérez",
            string email = "juan@test.com",
            string numberId = "1234567890",
            string phone = "0991234567",
            string gender = "Masculino",
            string idType = "Cédula",
            string patientType = "Nuevo",
            string currentUserId = "doctor-uid-1")
        {
            return new CreatePatientCommand(
                new CreatePatientByDoctorDto
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    NumberIdentification = numberId,
                    PhoneNumber = phone,
                    Gender = gender,
                    BirthDate = new DateOnly(1990, 1, 15),
                    IdentificationType = idType,
                    PatientType = patientType
                },
                currentUserId);
        }

        [Fact]
        public void Validate_ConDatosValidos_DebeSerValido()
        {
            var result = _validator.Validate(BuildCommand());
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        public void Validate_SinNombre_DebeFallar(string firstName)
        {
            var result = _validator.Validate(BuildCommand(firstName: firstName));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("FirstName"));
        }

        [Fact]
        public void Validate_NombreMayorA100Caracteres_DebeFallar()
        {
            var result = _validator.Validate(BuildCommand(firstName: new string('A', 101)));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("FirstName"));
        }

        [Theory]
        [InlineData("no-es-email")]
        [InlineData("sin-arroba.com")]
        public void Validate_EmailInvalido_DebeFallar(string email)
        {
            var result = _validator.Validate(BuildCommand(email: email));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("Email"));
        }

        [Fact]
        public void Validate_GeneroInvalido_DebeFallar()
        {
            var result = _validator.Validate(BuildCommand(gender: "Extraterrestre"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("Gender"));
        }

        [Fact]
        public void Validate_TipoIdentificacionInvalido_DebeFallar()
        {
            var result = _validator.Validate(BuildCommand(idType: "DNI"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("IdentificationType"));
        }

        [Fact]
        public void Validate_SinCurrentUserId_DebeFallar()
        {
            var result = _validator.Validate(BuildCommand(currentUserId: ""));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("CurrentUserId"));
        }

        [Fact]
        public void Validate_TiposPacienteValidos_DebeSerValido()
        {
            foreach (var tipo in new[] { "Nuevo", "Recurrente", "Referido", "Urgencias" })
            {
                var result = _validator.Validate(BuildCommand(patientType: tipo));
                result.IsValid.Should().BeTrue(because: $"'{tipo}' es un tipo de paciente válido");
            }
        }
    }
}
