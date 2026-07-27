using FluentAssertions;
using MedAnalyzer.Core.Application.Dto.MedicalRecord;
using MedAnalyzer.Core.Application.Features.MedicalRecords.Commands;
using MedAnalyzer.Core.Application.Features.MedicalRecords.Validators;

namespace MedAnalyzer.Tests.Validators
{
    public class CreateMedicalRecordCommandValidatorTests
    {
        private readonly CreateMedicalRecordCommandValidator _validator = new();

        private static MedicalRecordDto ValidDto() => new()
        {
            PatientId = 1,
            AppointmentId = 2,
            CreatedByUserId = "doctor-uid",
            DiagnosisInitial = "Hipertensión arterial leve",
            Notes = "Paciente con presión elevada",
            Antecedentes = "Sin antecedentes relevantes"
        };

        [Fact]
        public void Validate_ConDatosValidos_DebeSerValido()
        {
            var result = _validator.Validate(new CreateMedicalRecordCommand(ValidDto(), "uid"));
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_PatientIdCero_DebeFallar()
        {
            var dto = ValidDto();
            dto.PatientId = 0;
            var result = _validator.Validate(new CreateMedicalRecordCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("PatientId"));
        }

        [Fact]
        public void Validate_AppointmentIdCero_DebeFallar()
        {
            var dto = ValidDto();
            dto.AppointmentId = 0;
            var result = _validator.Validate(new CreateMedicalRecordCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("AppointmentId"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validate_SinDiagnostico_DebeFallar(string? diag)
        {
            var dto = ValidDto();
            dto.DiagnosisInitial = diag!;
            var result = _validator.Validate(new CreateMedicalRecordCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("DiagnosisInitial"));
        }

        [Fact]
        public void Validate_DiagnosticoMayorA500Caracteres_DebeFallar()
        {
            var dto = ValidDto();
            dto.DiagnosisInitial = new string('D', 501);
            var result = _validator.Validate(new CreateMedicalRecordCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validate_SinNotas_DebeFallar(string? notes)
        {
            var dto = ValidDto();
            dto.Notes = notes!;
            var result = _validator.Validate(new CreateMedicalRecordCommand(dto, "uid"));
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("Notes"));
        }

        [Fact]
        public void Validate_AntecedentesOpcionales_DebeSerValido()
        {
            var dto = ValidDto();
            dto.Antecedentes = null;
            dto.ObservacionesConsulta = null;
            var result = _validator.Validate(new CreateMedicalRecordCommand(dto, "uid"));
            result.IsValid.Should().BeTrue();
        }
    }
}
