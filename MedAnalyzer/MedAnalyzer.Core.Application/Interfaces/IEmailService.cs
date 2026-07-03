using MedAnalyzer.Core.Application.Dto.Email;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailRequestDto emailRequest);
    }
}