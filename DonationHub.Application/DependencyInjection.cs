using DonationHub.Application.Factories;
using DonationHub.Application.Features.Auth.Handlers;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DonationHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IModelFactory, ModelFactory>();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>(includeInternalTypes: true);

        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblyContaining<RegisterHandler>());

        return services;
    }
}
