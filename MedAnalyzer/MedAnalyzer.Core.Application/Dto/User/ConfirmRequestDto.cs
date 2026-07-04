namespace MedAnalyzer.Core.Application.Dto.User
{
    /// <summary>
    /// Parameters required to confirm a user account
    /// </summary>
    public class ConfirmRequestDto
    {
        /// <example>8f9d2a3b-5c4a-42a7-b6f7-0e9e8bdb178a</example>
        public required string UserId { get; set; }

        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
        public required string Token { get; set; }
    }
}
