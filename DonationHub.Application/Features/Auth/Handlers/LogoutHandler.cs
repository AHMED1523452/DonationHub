using DonationHub.Application.Features.Auth.Commands;
using DonationHub.Application.Interfaces;
using DonationHub.Application.Patterns;
using MediatR;

namespace DonationHub.Application.Features.Auth.Handlers
{
    public class LogoutHandler(ILoginService service) : IRequestHandler<LogoutCommand, GenericResult<string>>
    {
        private readonly ILoginService service = service;

        async Task<GenericResult<string>> IRequestHandler<LogoutCommand, GenericResult<string>>.Handle(LogoutCommand request, CancellationToken cancellationToken)
            => await service.LogoutAsync(cancellationToken);
    }

}
