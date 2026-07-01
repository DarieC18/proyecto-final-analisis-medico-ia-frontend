namespace MedAnalyzer.Core.Application.Dto.User
{
    public class UpdateUserDto
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required string NumberIdentification { get; set; }
        public string? Role { get; set; }
    }
}
