using HotelListing.API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddPresentationLayer()
    .AddAuthenticationLayer(builder.Configuration)
    .AddDataAccessLayer();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.ConfigureMiddleware();

app.Run();

/// <summary>
/// Extension method to register presentation layer services (API controllers, documentation).
/// </summary>
static class PresentationExtensions
{
    public static IServiceCollection AddPresentationLayer(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();

        return services;
    }
}

/// <summary>
/// Extension method to register authentication layer services (JWT Bearer authentication).
/// </summary>
static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthenticationLayer(this IServiceCollection services, IConfiguration config)
    {
        var jwtSettings = config.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("JWT SecretKey is not configured. Use 'dotnet user-secrets set \"JwtSettings:SecretKey\" \"your-32-char-key\"'");
        }

        var key = Encoding.ASCII.GetBytes(secretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // Signature validation
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                // Claims validation
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],

                // Expiration validation
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero  // No tolerance for expired tokens
            };
        });

        return services;
    }
}

/// <summary>
/// Extension method to register data access layer services (repositories).
/// </summary>
static class DataAccessExtensions
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services)
    {
        services.AddScoped<ICountryRepository, CountryRepository>();

        return services;
    }
}

/// <summary>
/// Extension method to configure the HTTP middleware pipeline.
/// </summary>
static class MiddlewareExtensions
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        // Development-only middleware
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // Security and routing middleware (order matters)
        app.UseHttpsRedirection();

        // ✅ Authentication BEFORE Authorization (CRITICAL: Order matters)
        app.UseAuthentication();      // Identify the user (extract JWT claims)
        app.UseAuthorization();       // Verify user permissions (check [Authorize] attributes)

        app.MapControllers();

        return app;
    }
}
