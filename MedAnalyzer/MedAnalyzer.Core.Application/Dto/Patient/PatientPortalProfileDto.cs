namespace MedAnalyzer.Core.Application.Dto.Patient
{
    public class PatientPortalProfileDto
    {
        public int PatientId { get; set; }
        public required string UserId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required string NumberIdentification { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string IdentificationType { get; set; } = string.Empty;
        public string PatientType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
