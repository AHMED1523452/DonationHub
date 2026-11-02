using DonationHub.Application.Interfaces;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Persistence;
using DonationHub.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DonationHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IHashingService, HashingService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IRegisterService, RegisterService>();
        services.AddTransient<ILoginService, LoginService>();
        services.AddTransient<IAuthMeService, AuthMeService>();
        services.AddTransient<IRefreshTokenService, RefreshTokenService>();
        services.AddTransient<ICreateAdminOrStaffService, CreateAdminOrStaffService>();

        services.AddScoped<SavingChagnesInterception>();

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            options.UseSqlServer(connectionString)
                   .AddInterceptors(serviceProvider.GetRequiredService<SavingChagnesInterception>()));

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        //. registeration for the Application user settings and password charaters rules, the email must be unique
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequiredUniqueChars = 0;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;

            options.User.RequireUniqueEmail = true; ///. Unique email   
        }).AddRoles<IdentityRole<Guid>>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

        services.AddTransient<IGenerateJwtToken, GenerateJwtToken>();

        return services;
    }
}
