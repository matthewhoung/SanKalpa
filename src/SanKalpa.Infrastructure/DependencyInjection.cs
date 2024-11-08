using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SanKalpa.Application.Abstrations.Authentication;
using SanKalpa.Application.Abstrations.Data;
using SanKalpa.Domain.Abstrations;
using SanKalpa.Domain.Services;
using SanKalpa.Domain.Users;
using SanKalpa.Infrastructure.Authentication;
using SanKalpa.Infrastructure.Data;
using SanKalpa.Infrastructure.Repositories;
using SanKalpa.Infrastructure.Services;
using System.Text;

namespace SanKalpa.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddPersistence(services, configuration);

        AddAuthentication(services, configuration);

        return services;
    }

    private static void AddPersistence(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = 
            configuration.GetConnectionString("Database") ??
            throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHashService, PasswordHashService>();
    }

    private static void AddAuthentication(
        IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure Authentication Options
        services.ConfigureOptions<AuthenticationOptionsSetup>();
        // Configure JWT Options
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        // Add Authentication with JWT Bearer
        var authenticationOptions = new AuthenticationOptions();
        var jwtOptions = new JwtOptions();

        configuration.GetSection(AuthenticationOptions.SectionName).Bind(authenticationOptions);
        configuration.GetSection(JwtOptions.SectionName).Bind(jwtOptions);

        // Add Authentication
        services.AddAuthorization();
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = authenticationOptions.DefaultScheme;
            options.DefaultChallengeScheme = authenticationOptions.DefaultChallengeScheme;
            options.DefaultAuthenticateScheme = authenticationOptions.DefaultAuthenticateScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        // Register services
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
    }
}
