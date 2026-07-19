using MedAnalyzer.Core.Application.Dto.Symptom;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.Symptoms.Commands
{
    public record CreateSymptomCommand(SymptomDto Dto, string CurrentUserId) : IRequest<SymptomDto?>;
    public class CreateSymptomCommandHandler : IRequestHandler<CreateSymptomCommand, SymptomDto?>
    {
        private readonly ISymptomService _service;
        public CreateSymptomCommandHandler(ISymptomService service) => _service = service;
        public async Task<SymptomDto?> Handle(CreateSymptomCommand req, CancellationToken ct)
            => await _service.CreateSymptom(req.Dto, req.CurrentUserId);
    }

    public record UpdateSymptomCommand(SymptomDto Dto, int Id) : IRequest<SymptomDto?>;
    public class UpdateSymptomCommandHandler : IRequestHandler<UpdateSymptomCommand, SymptomDto?>
    {
        private readonly ISymptomService _service;
        public UpdateSymptomCommandHandler(ISymptomService service) => _service = service;
        public async Task<SymptomDto?> Handle(UpdateSymptomCommand req, CancellationToken ct)
            => await _service.UpdateDtoAsync(req.Dto, req.Id);
    }
}
