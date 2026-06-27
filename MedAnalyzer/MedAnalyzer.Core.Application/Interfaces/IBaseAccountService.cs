using MedAnalyzer.Core.Application.Dto.User;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IBaseAccountService
    {
        Task<UserResponseDto> CambiarEstadoAsync(string id);
        Task<string> ConfirmAccountAsync(string userId, string token);
        Task<UserResponseDto> DeleteAsync(string id);
        Task<UserResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task<List<UserDto>> GetAllUser(bool? isActive = true);
        Task<UserDto?> GetUserByEmail(string email);
        Task<UserDto?> GetUserById(string Id);
        Task<UserDto?> GetUserByUserName(string userName);
        Task<RegisterResponseDto> RegisterAsync(RegisterDto dto, string? currentUserId = null);
        Task<UserResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<UserResponseDto> UpdateUserAsync(string id, UpdateUserDto dto);
    }
}