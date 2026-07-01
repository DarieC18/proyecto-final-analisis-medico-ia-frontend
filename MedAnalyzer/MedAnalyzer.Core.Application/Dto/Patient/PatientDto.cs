
using MedAnalyzer.Core.Application.Base;

namespace MedAnalyzer.Core.Application.Dto.Patient
{
    public class PatientDto : BaseDto<int>
    {
        public required string FullName { get; set; }
        public DateOnly BirthDate { get; set; }
        public required string Gender { get; set; }
        public required string PhoneNumber { get; set; }
        public required string IdentificationNumber { get; set; }
        public required string IdentificationType { get; set; }
        public required string PatientType { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
