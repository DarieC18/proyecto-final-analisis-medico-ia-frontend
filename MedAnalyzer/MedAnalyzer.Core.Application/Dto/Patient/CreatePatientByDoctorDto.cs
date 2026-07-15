namespace MedAnalyzer.Core.Application.Dto.Patient
{
    public class CreatePatientByDoctorDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string NumberIdentification { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public string IdentificationType { get; set; } = string.Empty;
        public string PatientType { get; set; } = string.Empty;
    }
}
