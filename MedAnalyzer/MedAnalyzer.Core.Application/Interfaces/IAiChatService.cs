using MedAnalyzer.Core.Application.Dto.AiChat;

namespace MedAnalyzer.Core.Application.Interfaces
{
    public interface IAiChatService
    {
        Task<AiChatResponseDto> AskAsync(AiChatRequestDto request, CancellationToken cancellationToken = default);
    }
}
