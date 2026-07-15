using MedAnalyzer.Core.Application.Dto.User;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IAccountServiceForWebApi : IBaseAccountService
    {
        Task<LoginResponseForApiDto> AuthenticateAsync(LoginDto loginDto);
        Task<(string UserId, string ResetToken)> RegisterPatientAccountAsync(
            string email, string firstName, string lastName, string numberIdentification);
    }
}
