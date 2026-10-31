using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DonationHub.Infrastructure.Persistence;

//. create internal instance without any hand from the DI Container
public static class DatabaseSeeder
{
    //. i used the IConfiguration here to avoid creating another class for seeding role or admin account
    public static async Task SeedAsync(IServiceProvider services, 
                                       IConfiguration configuration,
                                       CancellationToken cancellationToken = default)
    {
        //. life time for each scope 
        using var scope = services.CreateScope();

        //. getting the services from the DI Container 

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        
        var timeProvider = scope.ServiceProvider.GetRequiredService<TimeProvider>();

        var logger = scope.ServiceProvider
                          .GetRequiredService<ILoggerFactory>() //. abstraction dependency
                          .CreateLogger(nameof(DatabaseSeeder));               


        //. getting the section of Seed and Admin internal section 
        var adminSection = configuration.GetSection("Seed:Admin"); 
        var email = adminSection["Email"];
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentNullException(nameof(email));

        if (await userManager.FindByEmailAsync(email) is not null)
        {
            logger.LogInformation("Seed admin {Email} already exists, skipping.", email);
            return;
        }

        var password = configuration["Seed:Admin:Password"];
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException(
                "The admin seed password is missing. Set it with 'dotnet user-secrets set \"Seed:Admin:Password\" \"<password>\"'.");

        var preferredLanguage = Enum.TryParse<PreferredLanguage>(adminSection["PreferredLanguage"], true, out var language)
            ? language 
            : PreferredLanguage.en; //. as default

        var admin = new ApplicationUser
        {
            UserName = adminSection["UserName"] ?? email,
            Email = email,
            PhoneNumber = adminSection["PhoneNumber"],
            FullName = adminSection["FullName"] ?? email,
            preferredLanguage = preferredLanguage.ToString(),
            IsActive = true,
            EmailConfirmed = adminSection.GetValue("EmailConfirmed", true),
            PhoneNumberConfirmed = adminSection.GetValue("PhoneNumberConfirmed", true),
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime 
        };

        var createResult = await userManager.CreateAsync(admin, password);
        if (!createResult.Succeeded)
            throw new InvalidOperationException(
                $"Failed to seed admin '{email}': {string.Join("; ", createResult.Errors.Select(error => error.Description))}");

        var adminRole = adminSection["Role"] ?? "Admin"; 
        var addToRoleResult = await userManager.AddToRoleAsync(admin, adminRole);
        if (!addToRoleResult.Succeeded)
            throw new InvalidOperationException(
                $"Failed to assign role '{adminRole}' to '{email}': {string.Join("; ", addToRoleResult.Errors.Select(error => error.Description))}");

        logger.LogInformation("Seeded admin user {Email} with role {Role}.", email, adminRole);
    }
}
