using FluentAssertions;
using MedAnalyzer.Core.Application.Dto.Appointment;
using MedAnalyzer.Core.Application.Features.Appointments.Commands;
using MedAnalyzer.Core.Application.Features.Appointments.Validators;

namespace MedAnalyzer.Tests.Validators
{
    public class CreateAppointmentCommandValidatorTests
    {
        private readonly CreateAppointmentCommandValidator _validator = new();

        private static AppointmentDto ValidDto() => new()
        {
            Id = 0,
            PatientId = 1,
            DoctorId = "doctor-uid",
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Status = "Pending",
            Reason = "Control general"
        };

        [Fact]
        public void Validate_ConDatosValidos_DebeSerValido()
        {
            var result = _validator.Validate(new CreateAppointmentCommand(ValidDto(), "doctor-uid"));
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_PatientIdCero_DebeFallar()
        {
            var dto = ValidDto();
            dto.PatientId = 0;
            var result = _validator.Validate(new CreateAppointmentCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("PatientId"));
        }

        [Fact]
        public void Validate_FechaEnElPasado_DebeFallar()
        {
            var dto = ValidDto();
            dto.AppointmentDate = DateTime.UtcNow.AddDays(-1);
            var result = _validator.Validate(new CreateAppointmentCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("AppointmentDate"));
        }

        [Theory]
        [InlineData("")]
        public void Validate_SinMotivo_DebeFallar(string reason)
        {
            var dto = ValidDto();
            dto.Reason = reason;
            var result = _validator.Validate(new CreateAppointmentCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("Reason"));
        }

        [Theory]
        [InlineData("Cancelled")]
        [InlineData("InProgress")]
        [InlineData("Completed")]
        [InlineData("Pending")]
        public void Validate_EstadosValidos_DebeSerValido(string status)
        {
            var dto = ValidDto();
            dto.Status = status;
            var result = _validator.Validate(new CreateAppointmentCommand(dto, "uid"));
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EstadoInvalido_DebeFallar()
        {
            var dto = ValidDto();
            dto.Status = "Desconocido";
            var result = _validator.Validate(new CreateAppointmentCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("Status"));
        }

        [Fact]
        public void Validate_NotasMayorA1000Caracteres_DebeFallar()
        {
            var dto = ValidDto();
            dto.Notes = new string('X', 1001);
            var result = _validator.Validate(new CreateAppointmentCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
        }
    }

    public class ChangeAppointmentStatusCommandValidatorTests
    {
        private readonly ChangeAppointmentStatusCommandValidator _validator = new();

        [Theory]
        [InlineData("Pending")]
        [InlineData("InProgress")]
        [InlineData("Completed")]
        [InlineData("Cancelled")]
        public void Validate_EstadoValido_DebeSerValido(string status)
        {
            var result = _validator.Validate(new ChangeAppointmentStatusCommand(1, status, "uid"));
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_IdCero_DebeFallar()
        {
            var result = _validator.Validate(new ChangeAppointmentStatusCommand(0, "Pending", "uid"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Id");
        }

        [Fact]
        public void Validate_EstadoInvalido_DebeFallar()
        {
            var result = _validator.Validate(new ChangeAppointmentStatusCommand(1, "Borrado", "uid"));
            result.IsValid.Should().BeFalse();
        }
    }
}
