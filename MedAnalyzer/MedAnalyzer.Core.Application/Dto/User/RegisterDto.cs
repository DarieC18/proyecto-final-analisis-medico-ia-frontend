using Swashbuckle.AspNetCore.Annotations;

namespace MedAnalyzer.Core.Application.Dto.User
{
    public class RegisterDto
    {
        [SwaggerParameter(Description = "Name is required")]
        public required string Name { get; set; }

        [SwaggerParameter(Description = "LastName is required")]
        public required string LastName { get; set; }

        [SwaggerParameter(Description = "Email is required")]
        public required string Email { get; set; }

        [SwaggerParameter(Description = "UserName is required")]
        public required string UserName { get; set; }

        [SwaggerParameter(Description = "number identification is required")]
        public required string NumberIdentification { get; set; }

        [SwaggerParameter(Description = "Password is required")]
        public required string Password { get; set; }

        [SwaggerParameter(Description = "ConfirmPassword is required")]
        public required string ConfirmPassword { get; set; }

        public string? Role { get; set; }  
    }
}
