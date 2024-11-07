using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace SanKalpa.Infrastructure.Authentication;

public class JwtConfiguration : IConfigureOptions<JwtOptions>
{
    private readonly IConfiguration _configuration;

    public JwtConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(JwtOptions options)
    {
        _configuration.GetSection("Jwt").Bind(options);
    }
}
