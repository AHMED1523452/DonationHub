using DonationHub.Application.Patterns;
using MediatR;

namespace DonationHub.Application.Features.Category.Commands
{
    public sealed record DeleteCategoryCommand(Guid Id) : IRequest<GenericResult<string>>;
}
