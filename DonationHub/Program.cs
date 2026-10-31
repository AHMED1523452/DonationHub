using DonationHub.Application;
using DonationHub.Application.Interfaces;
using DonationHub.Infrastructure;
using DonationHub.Infrastructure.Persistence;
using DonationHub.Infrastructure.Security;
using DonationHub.Infrastructure.Services;
using DonationHub.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var argsList = Environment.GetCommandLineArgs();
var runningEf = argsList.Any(a => a.Equals("ef", StringComparison.OrdinalIgnoreCase) ||
                                  a.IndexOf("ef", StringComparison.OrdinalIgnoreCase) >= 0);

if (!runningEf && builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {

        //. Include XML comments if the XML documentation file exists
        var xmlPath = Path.Combine(
            AppContext.BaseDirectory,
            $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml");

        //. Check if the XML documentation file exists before including it
        if (File.Exists(xmlPath))
            options.IncludeXmlComments(xmlPath);

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter your JWT bearer token."
        });

        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
    });
}

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

//. Registeration for time provider built in class to provide the utc now
//. creating one obect or instace for the whole project life 
builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

builder.Services.AddLogging();
builder.Services.AddTransient<GlobalExceptionMiddleware>();

//. Configuration for Jwt Options 
builder.Services
    .AddOptions<JWTSettings>()
    .Bind(builder.Configuration.GetSection("JwtSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

//.Default Scheme 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKEY"])),
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = ClaimTypes.Role
    };
});

//. register the HttpContext Accessor to let us get the current user in any place
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<ICurrentUserService, CurrentUserService>();

builder.Services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

await DatabaseSeeder.SeedAsync(app.Services, app.Configuration);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
    app.MapScalarApiReference(); //. we will use scalar to test the APIs 
}

app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
