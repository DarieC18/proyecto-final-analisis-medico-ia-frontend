using FluentAssertions;
using MedAnalyzer.Core.Application.Dto.Symptom;
using MedAnalyzer.Core.Application.Features.Symptoms.Commands;
using MedAnalyzer.Core.Application.Features.Symptoms.Validators;

namespace MedAnalyzer.Tests.Validators
{
    public class CreateSymptomCommandValidatorTests
    {
        private readonly CreateSymptomCommandValidator _validator = new();

        private static SymptomDto ValidDto() => new()
        {
            AppointmentId = 1,
            Name = "Cefalea",
            Severity = "Leve",
            StartedAt = DateOnly.FromDateTime(DateTime.Today),
            Notes = "Dolor de cabeza leve"
        };

        [Fact]
        public void Validate_ConDatosValidos_DebeSerValido()
        {
            var result = _validator.Validate(new CreateSymptomCommand(ValidDto(), "uid"));
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_AppointmentIdCero_DebeFallar()
        {
            var dto = ValidDto();
            dto.AppointmentId = 0;
            var result = _validator.Validate(new CreateSymptomCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("AppointmentId"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validate_SinNombre_DebeFallar(string? nombre)
        {
            var dto = ValidDto();
            dto.Name = nombre!;
            var result = _validator.Validate(new CreateSymptomCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("Name"));
        }

        [Theory]
        [InlineData("Leve")]
        [InlineData("Moderado")]
        [InlineData("Severo")]
        public void Validate_SeveridadesValidas_DebeSerValido(string severity)
        {
            var dto = ValidDto();
            dto.Severity = severity;
            var result = _validator.Validate(new CreateSymptomCommand(dto, "uid"));
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("Extremo")]
        [InlineData("Crítico")]
        public void Validate_SeveridadInvalida_DebeFallar(string severity)
        {
            var dto = ValidDto();
            dto.Severity = severity;
            var result = _validator.Validate(new CreateSymptomCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("Severity"));
        }

        [Fact]
        public void Validate_NotasMayorA500Caracteres_DebeFallar()
        {
            var dto = ValidDto();
            dto.Notes = new string('N', 501);
            var result = _validator.Validate(new CreateSymptomCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
        }
    }
}
