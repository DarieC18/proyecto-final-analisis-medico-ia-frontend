namespace MedAnalyzer.Core.Application.Dto.User
{
    /// <summary>
    /// Parameters required to request a password reset token
    /// </summary>
    public class ForgotPasswordApiRequestDto
    {
        /// <example>juanp</example>
        public required string UserName { get; set; }
    }
}
