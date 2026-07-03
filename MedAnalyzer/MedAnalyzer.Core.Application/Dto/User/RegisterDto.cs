namespace MedAnalyzer.Core.Application.Dto.User
{
    public class RegisterDto
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required string NumberIdentification { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public string? Role { get; set; }
    }
}
