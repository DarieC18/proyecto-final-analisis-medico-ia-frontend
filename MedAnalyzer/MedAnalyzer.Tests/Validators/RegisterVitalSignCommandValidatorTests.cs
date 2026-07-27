using FluentAssertions;
using MedAnalyzer.Core.Application.Dto.VitalSign;
using MedAnalyzer.Core.Application.Features.VitalSigns.Commands;
using MedAnalyzer.Core.Application.Features.VitalSigns.Validators;

namespace MedAnalyzer.Tests.Validators
{
    public class RegisterVitalSignCommandValidatorTests
    {
        private readonly RegisterVitalSignCommandValidator _validator = new();

        private static VitalSignDto ValidDto() => new()
        {
            AppointmentId = 1,
            Temperature = 36.5m,
            HeartRate = 75,
            SystolicPressure = 120,
            DiastolicPressure = 80,
            OxygenSaturation = 98m,
            Glucose = 90m
        };

        [Fact]
        public void Validate_ConTodosLosSignosValidos_DebeSerValido()
        {
            var result = _validator.Validate(new RegisterVitalSignCommand(ValidDto(), "uid"));
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_SoloUnSignoRegistrado_DebeSerValido()
        {
            var dto = new VitalSignDto { AppointmentId = 1, Temperature = 36.5m };
            var result = _validator.Validate(new RegisterVitalSignCommand(dto, "uid"));
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_SinNingunSigno_DebeSerValido()
        {
            var dto = new VitalSignDto { AppointmentId = 1 };
            var result = _validator.Validate(new RegisterVitalSignCommand(dto, "uid"));
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_AppointmentIdCero_DebeFallar()
        {
            var dto = ValidDto();
            dto.AppointmentId = 0;
            var result = _validator.Validate(new RegisterVitalSignCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("AppointmentId"));
        }

        [Theory]
        [InlineData(29.9)]
        [InlineData(45.1)]
        public void Validate_TemperaturaFueraDeRango_DebeFallar(double temp)
        {
            var dto = new VitalSignDto { AppointmentId = 1, Temperature = (decimal)temp };
            var result = _validator.Validate(new RegisterVitalSignCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("Temperature"));
        }

        [Theory]
        [InlineData(19)]
        [InlineData(301)]
        public void Validate_FrecuenciaCardiacaFueraDeRango_DebeFallar(int hr)
        {
            var dto = new VitalSignDto { AppointmentId = 1, HeartRate = hr };
            var result = _validator.Validate(new RegisterVitalSignCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("HeartRate"));
        }

        [Theory]
        [InlineData(49.9)]
        [InlineData(100.1)]
        public void Validate_SaturacionFueraDeRango_DebeFallar(double sat)
        {
            var dto = new VitalSignDto { AppointmentId = 1, OxygenSaturation = (decimal)sat };
            var result = _validator.Validate(new RegisterVitalSignCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("OxygenSaturation"));
        }

        [Fact]
        public void Validate_SaturacionLimiteInferior_DebeSerValido()
        {
            var dto = new VitalSignDto { AppointmentId = 1, OxygenSaturation = 50m };
            var result = _validator.Validate(new RegisterVitalSignCommand(dto, "uid"));
            result.IsValid.Should().BeTrue();
        }
    }
}
