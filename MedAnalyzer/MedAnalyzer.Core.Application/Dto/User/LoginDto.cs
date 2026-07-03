namespace MedAnalyzer.Core.Application.Dto.User
{ 
    public class LoginDto
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
