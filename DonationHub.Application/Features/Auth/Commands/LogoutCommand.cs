using DonationHub.Application.Patterns;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Auth.Commands
{
    public sealed record LogoutCommand : IRequest<GenericResult<string>>;
}
