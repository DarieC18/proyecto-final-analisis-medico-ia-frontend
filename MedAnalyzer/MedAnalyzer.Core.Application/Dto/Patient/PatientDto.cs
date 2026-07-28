using MedAnalyzer.Core.Application.Base;

namespace MedAnalyzer.Core.Application.Dto.Patient
{
    public class PatientDto : BaseDto<int>
    {
        public required string UserId { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string IdentificationType { get; set; } = string.Empty;
        public string PatientType { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
