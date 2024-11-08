using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace SanKalpa.Infrastructure.Authentication;

public class AuthenticationOptionsSetup : IConfigureOptions<AuthenticationOptions>
{
    private readonly IConfiguration _configuration;

    public AuthenticationOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(AuthenticationOptions options)
    {
        _configuration.GetSection(AuthenticationOptions.SectionName).Bind(options);
    }
}
