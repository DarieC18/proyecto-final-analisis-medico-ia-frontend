namespace MedAnalyzer.Core.Application.Dto.User
{
    public class DoctorListItemDto
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public string? Specialty { get; set; }
    }
}
