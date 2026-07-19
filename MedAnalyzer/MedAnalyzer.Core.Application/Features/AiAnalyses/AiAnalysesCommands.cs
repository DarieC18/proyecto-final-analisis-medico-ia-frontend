using MedAnalyzer.Core.Application.Dto.AiAnalisys;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.AiAnalyses.Commands
{
    public record GenerateAiAnalysisCommand(GenerateAiAnalysisRequestDto Request, string CurrentUserId) : IRequest<AiAnalisysDto>;
    public class GenerateAiAnalysisCommandHandler : IRequestHandler<GenerateAiAnalysisCommand, AiAnalisysDto>
    {
        private readonly IAiAnalisysServices _service;
        public GenerateAiAnalysisCommandHandler(IAiAnalisysServices service) => _service = service;
        public async Task<AiAnalisysDto> Handle(GenerateAiAnalysisCommand req, CancellationToken ct)
            => await _service.GenerateAnalysisAsync(req.Request, req.CurrentUserId);
    }

    public record MarkAiAnalysisReviewedCommand(int Id) : IRequest<bool>;
    public class MarkAiAnalysisReviewedCommandHandler : IRequestHandler<MarkAiAnalysisReviewedCommand, bool>
    {
        private readonly IAiAnalisysServices _service;
        public MarkAiAnalysisReviewedCommandHandler(IAiAnalisysServices service) => _service = service;
        public async Task<bool> Handle(MarkAiAnalysisReviewedCommand req, CancellationToken ct)
        {
            var dto = await _service.GetDtoById(req.Id);
            if (dto == null) return false;
            dto.IsReviewed = true;
            await _service.UpdateDtoAsync(dto, req.Id);
            return true;
        }
    }
}
