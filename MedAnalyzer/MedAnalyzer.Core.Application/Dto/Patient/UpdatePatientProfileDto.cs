namespace MedAnalyzer.Core.Application.Dto.Patient
{
    public class UpdatePatientProfileDto
    {
        public DateOnly BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string IdentificationType { get; set; } = string.Empty;
        public string PatientType { get; set; } = string.Empty;
    }
}
