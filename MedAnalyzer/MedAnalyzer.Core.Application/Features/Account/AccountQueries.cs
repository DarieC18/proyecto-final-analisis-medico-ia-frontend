using MedAnalyzer.Core.Application.Dto.User;
using MedAnalyzer.Core.Application.Interfaces;
using MediatR;

namespace MedAnalyzer.Core.Application.Features.Account.Queries
{
    public record GetAllUsersQuery : IRequest<List<UserDto>>;
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
    {
        private readonly IAccountServiceForWebApi _service;
        public GetAllUsersQueryHandler(IAccountServiceForWebApi service) => _service = service;
        public async Task<List<UserDto>> Handle(GetAllUsersQuery _, CancellationToken ct)
            => await _service.GetAllUser(null);
    }
}
